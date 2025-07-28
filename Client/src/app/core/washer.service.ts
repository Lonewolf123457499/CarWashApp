import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WasherService {
  private readonly apiUrl = 'http://localhost:5050/api/Washer';

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  getPendingOrders(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/pending-orders`);
  }

  getAllOrders(): Observable<any[]> {
    return this.http.get<any[]>(`http://localhost:5050/api/Washer/my-orders`);
  }

  acceptOrder(orderId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/accept-order/${orderId}`, null, { headers: this.getAuthHeaders() });
  }

  startService(orderId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/start-work/${orderId}`, null, { headers: this.getAuthHeaders() });
  }

  completeService(orderId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/complete-order/${orderId}`, null, { headers: this.getAuthHeaders(), responseType: 'text' });
  }

  getProfile(): Observable<any> {
    return this.http.get(`${this.apiUrl}/profile`);
  }

  updateProfile(profileData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/profile`, profileData, { responseType: 'text' });
  }
}