import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  private readonly washpackagesUrl = `http://localhost:5050/api/Admin/washpackages`;

  private readonly addonsUrl = `http://localhost:5050/api/Admin/addons`;


  constructor(private http: HttpClient) { }

   getWashPackages(): Observable<any[]> {
    return this.http.get<any[]>(this.washpackagesUrl);
  }

 getaddons(): Observable<any[]> {
    return this.http.get<any[]>(this.addonsUrl);
}
}