import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private readonly apiUrl = 'https://localhost:7297/api/order';

  constructor(private http: HttpClient) { }

  // Order History
  getOrderHistory(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/history`);
  }

  // Get specific order
  getOrderById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${id}`);
  }

  // Admin only endpoints
  getOrdersByCustomer(customerId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/customer/${customerId}`);
  }

  getOrdersByWasher(washerId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/washer/${washerId}`);
  }

  getAllOrders(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/all`);
  }
}