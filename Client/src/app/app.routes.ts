import { Routes } from '@angular/router';
import { LandingComponent } from './landing/landing.component';
import { LoginComponent } from './authComponent/login/login.component';
import { RegisterComponent } from './authComponent/register/register.component';
import { CustomerDashboardComponent } from './customer-dashboard/customer-dashboard.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { WasherDashboardComponent } from './washer-dashboard/washer-dashboard.component';

export const routes: Routes = [
  { path: '', component: LandingComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'customer-dashboard', component: CustomerDashboardComponent },
  { path: 'admin-dashboard', component: AdminDashboardComponent },
  { path: 'washer-dashboard', component: WasherDashboardComponent },
  { path: '**', redirectTo: '' }
];
