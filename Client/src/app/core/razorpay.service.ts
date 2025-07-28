import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

declare var Razorpay: any;

@Injectable({
  providedIn: 'root'
})
export class RazorpayService {
  private readonly apiUrl = 'http://localhost:5050/api';

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  createOrder(orderData: any): Observable<any> {
    console.log('Creating order with data:', orderData);
    return this.http.post(`${this.apiUrl}/Payment/create-order`, orderData);
  }

  verifyPayment(paymentData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Payment/verify`, paymentData, { headers: this.getAuthHeaders() });
  }

  processPayment(orderId: number, userProfile: any, orderAmount: number): Promise<any> {
    return new Promise((resolve, reject) => {
      // First create order on backend
      console.log('Creating payment order:', {
        orderId: orderId,
        amount: orderAmount,
        userProfile: userProfile
      });
      
      this.createOrder({ 
        orderId: orderId,
        amount: Math.round(orderAmount * 100) / 100 // Ensure 2 decimal places
      }).subscribe({
        next: (orderResponse) => {
          console.log('Backend order response:', orderResponse);
          const options = {
            key: 'rzp_test_x4I9ge6i9FHstL',
            amount: orderResponse.amount || (orderAmount * 100), // Fallback to passed amount in paise
            currency: orderResponse.currency || 'INR',
            name: 'Green Car Wash',
            description: `Payment for Order #${orderId}`,
            order_id: orderResponse.orderId,
            handler: (response: any) => {
              // Verify payment on backend
              this.verifyPayment({
                OrderId: orderId,
                RazorpayOrderId: response.razorpay_order_id,
                RazorpayPaymentId: response.razorpay_payment_id,
                RazorpaySignature: response.razorpay_signature
              }).subscribe({
                next: (verifyResponse) => {
                  resolve(verifyResponse);
                },
                error: (verifyError) => {
                  reject(verifyError);
                }
              });
            },
            prefill: {
              name: userProfile?.fullName || 'Customer',
              email: userProfile?.email || 'customer@example.com',
              contact: userProfile?.phone || '9876543210',
            },
            theme: {
              color: '#10B981',
            },
            modal: {
              ondismiss: () => {
                reject(new Error('Payment cancelled by user'));
              }
            }
          };

          const rzp = new Razorpay(options);
          rzp.open();
        },
        error: (orderError) => {
          console.error('Create order failed:', orderError);
          console.error('Error details:', orderError.error);
          console.error('Request data was:', { orderId: orderId, amount: orderAmount });
          reject(orderError);
        }
      });
    });
  }
}