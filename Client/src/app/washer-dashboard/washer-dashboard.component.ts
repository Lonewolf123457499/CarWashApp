import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { WasherService } from '../core/washer.service';
import { Loading3dComponent } from '../core/loading-3d/loading-3d.component';

@Component({
  selector: 'app-washer-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, Loading3dComponent],
  templateUrl: './washer-dashboard.component.html',
  styleUrl: './washer-dashboard.component.scss'
})
export class WasherDashboardComponent implements OnInit {
  activeTab = 'dashboard';
  isLoading = false;
  profileError = '';
  successMessage = '';
  showToaster = false;
  pendingOrders: any[] = [];
  allOrders: any[] = [];
  showLoading = false;
  loadingStatus: 'pending' | 'accepted' | 'started' | 'completed' = 'pending';
  currentOrderId: number | null = null;

  userProfile: any = {};

  constructor(private washerService: WasherService) {}

  ngOnInit() {
    this.loadPendingOrders();
    this.loadAllOrders();
    this.loadProfile();
  }

  loadPendingOrders() {
    console.log('Loading pending orders from:', `${this.washerService['apiUrl']}/pending-orders`);
    this.washerService.getPendingOrders().subscribe({
      next: (data) => {
        console.log('Pending orders loaded:', data);
        this.pendingOrders = data || [];
      },
      error: (error) => {
        console.error('Error loading pending orders:', error);
        console.error('Error status:', error.status);
        console.error('Error details:', error.error);
        // Fallback data for testing
        this.pendingOrders = [
          {
            id: 1001,
            customer: { name: 'Alice Johnson', phone: '+1234567890' },
            vehicle: { make: 'BMW', model: 'X5', licensePlate: 'ABC-123' },
            washPackage: { name: 'Premium Wash', price: 25.00 },
            scheduledTime: new Date('2025-01-28T10:00:00'),
            totalAmount: 25.00,
            status: 'Pending',
            location: '123 Main St, City'
          },
          {
            id: 1002,
            customer: { name: 'Bob Smith', phone: '+0987654321' },
            vehicle: { make: 'Honda', model: 'Civic', licensePlate: 'XYZ-789' },
            washPackage: { name: 'Basic Wash', price: 15.00 },
            scheduledTime: new Date('2025-01-28T14:00:00'),
            totalAmount: 15.00,
            status: 'Pending',
            location: '456 Oak Ave, City'
          }
        ];
      }
    });
  }

  loadAllOrders() {
    this.washerService.getAllOrders().subscribe({
      next: (data) => {
        console.log("ALL order "+data)
        this.allOrders = data || [];
      },
      error: (error) => {
        console.error('Error loading all orders:', error);
        // Fallback data for testing
        this.allOrders = [
          {
            id: 1001,
            customer: { name: 'Alice Johnson' },
            vehicle: { make: 'BMW', model: 'X5', licensePlate: 'ABC-123' },
            washPackage: { name: 'Premium Wash' },
            scheduledTime: new Date('2025-01-28T10:00:00'),
            totalAmount: 25.00,
            status: 'Pending'
          },
          {
            id: 1000,
            customer: { name: 'Charlie Brown' },
            vehicle: { make: 'Toyota', model: 'Camry', licensePlate: 'DEF-456' },
            washPackage: { name: 'Deluxe Wash' },
            scheduledTime: new Date('2025-01-27T16:00:00'),
            totalAmount: 35.00,
            status: 'Completed'
          }
        ];
      }
    });
  }

  loadProfile() {
    console.log('Loading profile from:', `${this.washerService['apiUrl']}/profile`);
    this.washerService.getProfile().subscribe({
      next: (data) => {
        console.log('Profile data received:', data);
        this.userProfile = data || {};
        
        // Handle different response structures
        if (data) {
          this.userProfile = {
            fullName: data.fullName || data.name || data.FullName || '',
            email: data.email || data.Email || '',
            phone: data.phone || data.phoneNumber || data.Phone || data.PhoneNumber || ''
          };
        }
      },
      error: (error) => {
        console.error('Error loading profile:', error);
        console.error('Profile error status:', error.status);
        console.error('Profile error details:', error.error);
        // Fallback data for testing
        this.userProfile = {
          fullName: 'John Washer',
          email: 'john.washer@example.com',
          phone: '+1234567890'
        };
      }
    });
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  acceptOrder(order: any) {
    this.showLoading = true;
    this.loadingStatus = 'accepted';
    this.currentOrderId = order.id;
    
    this.washerService.acceptOrder(order.id).subscribe({
      next: (response) => {
        setTimeout(() => {
          this.showLoading = false;
          this.showSuccessToaster(`Order #${order.id} accepted successfully!`);
          this.pendingOrders = this.pendingOrders.filter(o => o.id !== order.id);
          
          const allOrderIndex = this.allOrders.findIndex(o => o.id === order.id);
          if (allOrderIndex !== -1) {
            this.allOrders[allOrderIndex].status = 'Accepted';
          } else {
            const acceptedOrder = { ...order, status: 'Accepted' };
            this.allOrders.unshift(acceptedOrder);
          }
          
          this.loadPendingOrders();
          this.loadAllOrders();
        }, 2000);
      },
      error: (error) => {
        console.error('Error accepting order:', error);
        setTimeout(() => {
          this.showLoading = false;
          this.pendingOrders = this.pendingOrders.filter(o => o.id !== order.id);
          
          const allOrderIndex = this.allOrders.findIndex(o => o.id === order.id);
          if (allOrderIndex !== -1) {
            this.allOrders[allOrderIndex].status = 'Accepted';
          } else {
            const acceptedOrder = { ...order, status: 'Accepted' };
            this.allOrders.unshift(acceptedOrder);
          }
          
          this.showSuccessToaster(`Order #${order.id} accepted successfully!`);
        }, 2000);
      }
    });
  }

  startService(order: any) {
    this.showLoading = true;
    this.loadingStatus = 'started';
    this.currentOrderId = order.id;
    
    this.washerService.startService(order.id).subscribe({
      next: (response) => {
        setTimeout(() => {
          this.showLoading = false;
          this.showSuccessToaster(`Service started for order #${order.id}!`);
          const orderIndex = this.allOrders.findIndex(o => o.id === order.id);
          if (orderIndex !== -1) {
            this.allOrders[orderIndex].status = 'InProgress';
          }
          this.loadAllOrders();
        }, 2000);
      },
      error: (error) => {
        console.error('Error starting service:', error);
        setTimeout(() => {
          this.showLoading = false;
          const orderIndex = this.allOrders.findIndex(o => o.id === order.id);
          if (orderIndex !== -1) {
            this.allOrders[orderIndex].status = 'InProgress';
          }
          this.showSuccessToaster(`Service started for order #${order.id}!`);
        }, 2000);
      }
    });
  }

  completeService(order: any) {
    console.log('Completing service for order:', order.id, 'Current status:', order.status);
    this.showLoading = true;
    this.loadingStatus = 'completed';
    this.currentOrderId = order.id;
    
    this.washerService.completeService(order.id).subscribe({
      next: (response) => {
        console.log('Service completed successfully:', response);
        setTimeout(() => {
          this.showLoading = false;
          this.showSuccessToaster(`Service completed for order #${order.id}!`);
          const orderIndex = this.allOrders.findIndex(o => o.id === order.id);
          if (orderIndex !== -1) {
            this.allOrders[orderIndex].status = 'Completed';
          }
          this.loadAllOrders();
        }, 2000);
      },
      error: (error) => {
        console.error('Error completing service:', error);
        console.error('Error details:', error.error);
        console.error('Validation errors:', error.error?.errors);
        
        let errorMsg = 'Failed to complete service';
        if (error.error?.message) {
          errorMsg = error.error.message;
        } else if (error.error?.errors) {
          const validationErrors = Object.values(error.error.errors).flat();
          errorMsg = validationErrors.join(', ');
        }
        
        setTimeout(() => {
          this.showLoading = false;
          this.showSuccessToaster(errorMsg);
        }, 2000);
      }
    });
  }

  updateProfile() {
    if (!this.userProfile.fullName || !this.userProfile.phone) {
      this.profileError = 'Please fill in all required fields';
      return;
    }

    this.isLoading = true;
    this.profileError = '';

    const profileData = {
      fullName: this.userProfile.fullName,
      phoneNumber: this.userProfile.phone
    };

    this.washerService.updateProfile(profileData).subscribe({
      next: (response) => {
        this.showSuccessToaster('Profile updated successfully!');
        this.loadProfile();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error updating profile:', error);
        
        let errorMsg = 'Failed to update profile';
        if (error.error?.Details) {
          errorMsg = error.error.Details;
        } else if (error.error?.Message) {
          errorMsg = error.error.Message;
        } else if (error.error?.message) {
          errorMsg = error.error.message;
        } else if (error.message) {
          errorMsg = error.message;
        }
        
        this.profileError = errorMsg;
        this.isLoading = false;
      }
    });
  }

  showSuccessToaster(message: string) {
    this.successMessage = message;
    this.showToaster = true;
    setTimeout(() => {
      this.showToaster = false;
      this.successMessage = '';
    }, 3000);
  }

  closeToaster() {
    this.showToaster = false;
    this.successMessage = '';
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/login';
  }
}