import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {

  constructor(http:HttpClient) { }

   url=`http://localhost:5050/api/Customer/vehicles`

   getAllvehicle()
   {
    this
   }

}
