import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private readonly apiUrl = 'http://localhost:5050/api/Admin';

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  // Dashboard Stats
  getDashboardStats(): Observable<any> {
    return this.http.get(`${this.apiUrl}/dashboard`, { headers: this.getAuthHeaders() });
  }

  // Wash Packages CRUD
  getWashPackages(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/washpackages`);
  }

  createWashPackage(packageData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/washpackage`, packageData);
  }

  updateWashPackage(id: number, packageData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/washpackage/${id}`, packageData,{responseType: 'text'});
  }

  deleteWashPackage(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/washpackage/${id}`,{responseType: 'text'});
  }

  // Addons CRUD
  getAddons(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/addons`);
  }

  createAddon(addonData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/addons`, addonData);
  }

  updateAddon(id: number, addonData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/addons/${id}`, addonData);
  }

  deleteAddon(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/addons/${id}`);
  }

  // Washers Management
  getWashers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/washers`, { headers: this.getAuthHeaders() });
  }

  updateWasherStatus(id: number, statusData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/washer/${id}/status`, statusData,{responseType: 'text'});
  }

  // Customers Management
  getCustomers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/customers`, { headers: this.getAuthHeaders() });
  }

  // Orders Management
  getAllOrders(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/orders`, { headers: this.getAuthHeaders() });
  }
}