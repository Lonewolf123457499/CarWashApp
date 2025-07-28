import { Component } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/authservice.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  fullName = '';
  email = '';
  phone = '';
  password = '';
  selectedRole: 'Customer' | 'Washer' = 'Customer';
  isLoading = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  selectRole(role: 'Customer' | 'Washer') {
    this.selectedRole = role;
    this.errorMessage = '';
  }

  onSubmit() {
    this.isLoading = true;
    this.errorMessage = '';
    
    const formData = {
      name: this.fullName,
      email: this.email,
      phoneNumber: this.phone,
      password: this.password,
      role: this.selectedRole
    };
    
    console.log('Submitting form data:', formData);
    
    const registerObservable = this.selectedRole === 'Customer' 
      ? this.authService.registerCustomer(formData)
      : this.authService.registerWasher(formData);
    
    registerObservable.subscribe({
      next: (response: any) => {
        console.log('Registration successful:', response);
        
        // Navigate to appropriate dashboard based on role
        switch (this.selectedRole) {
          case 'Customer':
            this.router.navigate(['/customer-dashboard']);
            break;
          case 'Washer':
            this.router.navigate(['/washer-dashboard']);
            break;
          default:
            this.router.navigate(['/login']);
        }
      },
      error: (error) => {
        console.error('Registration failed:', error);
        console.error('Error details:', error.error);
        
        if (error.error?.errors) {
          // Show validation errors
          const errors = Object.values(error.error.errors).flat();
          this.errorMessage = errors.join(', ');
        } else {
          this.errorMessage = error.error?.message || 'Registration failed. Please try again.';
        }
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }
}
