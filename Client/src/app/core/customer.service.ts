import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface VehicleDTO {
  make: string;
  model: string;
  numberPlate: string;
  imageUrl: string;
}

export interface OrderDTO {
  vehicleId: number;
  washPackageId: number;
  orderDate: Date;
  addonIds?: number[];
}

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly apiUrl = 'http://localhost:5050/api/Customer';

  private readonly washpackagesUrl = `http://localhost:5050/api/Admin/washpackages`;

  private readonly addonsUrl = `http://localhost:5050/api/Admin/addons`;

  constructor(private http: HttpClient) { }


  // Profile Management
  getProfile(): Observable<any> {
    return this.http.get(`${this.apiUrl}/profile`);
  }

  updateProfile(profileData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/profile`, profileData, { responseType: 'text' });
  }

  // Vehicle Management
  getVehicles(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/vehicles`);
  }

  getVehicle(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/vehicle/${id}`);
  }

  addVehicle(vehicle: VehicleDTO): Observable<any> {
    return this.http.post(`${this.apiUrl}/vehicle`, vehicle);
  }

  // deleteVehicle(id: number): Observable<any> {
  //   return this.http.delete(`${this.apiUrl}/vehicle/${id}`);
  // }

  // Order Management
  placeOrder(order: OrderDTO): Observable<any> {
    return this.http.post(`${this.apiUrl}/order`, order);
  }

  getOrders(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/orders`);
  }

  getReceipt(orderId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/receipt/${orderId}`);
  }

  submitRating(ratingData: any): Observable<any> {
    console.log('Submitting rating:', ratingData);
    return this.http.post(`${this.apiUrl}/rating`, ratingData,{responseType: 'text'});
  }

  getRatings(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/ratings`);
  }
}