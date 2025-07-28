import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CustomerService } from '../core/customer.service';
import { SharedService } from '../core/shared.service';
import { RazorpayService } from '../core/razorpay.service';

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './customer-dashboard.component.html',
  styleUrl: './customer-dashboard.component.scss'
})
export class CustomerDashboardComponent implements OnInit {
  activeTab = 'dashboard';
  showAddVehicleModal = false;
  showPlaceOrderModal = false;
  showPaymentModal = false;
  showReceiptModal = false;
  showRatingModal = false;
  selectedOrderForPayment: any = null;
  selectedReceipt: any = null;
  selectedOrderForRating: any = null;
  rating = { rating: 5, comment: '' };
  ratedOrders: Set<number> = new Set();

  // Data properties
  vehicles: any[] = [];
  orders: any[] = [];
  washPackages: any[] = [];
  addons: any[] = [];
  userProfile: any = {};
  isLoading = false;
  errorMessage = '';
  addVehicleError = '';
  placeOrderError = '';
  paymentError = '';
  ratingError = '';
  profileError = '';
  successMessage = '';
  showToaster = false;

  newVehicle = { make: '', model: '', numberPlate: '', imageUrl: '' };
  newOrder = { vehicleId: '', washPackageId: '', orderDate: '', addonIds: [] as number[] };

  constructor(
    private customerService: CustomerService,
    private sharedService: SharedService,
    private razorpayService: RazorpayService
  ) {}

  ngOnInit() {
    this.loadUserProfile();
    this.loadVehicles();
    this.loadOrders();
    this.loadWashPackages();
    this.loadAddons();
    this.loadRatings();
    this.checkPaymentStatus();
  }

  checkPaymentStatus() {
    const urlParams = new URLSearchParams(window.location.search);
    const paymentStatus = urlParams.get('payment');
    
    if (paymentStatus === 'success') {
      this.showSuccessToaster('Payment completed successfully!');
      this.loadOrders(); // Refresh orders to show updated status
      // Clean up URL
      window.history.replaceState({}, document.title, window.location.pathname);
    } else if (paymentStatus === 'cancelled') {
      this.errorMessage = 'Payment was cancelled.';
      setTimeout(() => this.errorMessage = '', 5000);
      // Clean up URL
      window.history.replaceState({}, document.title, window.location.pathname);
    }
  }

  loadVehicles() {
    this.customerService.getVehicles().subscribe({
      next: (data) => {
        console.log(this.vehicles);
        this.vehicles = data;
      },
      error: (error) => {
        console.error('Error loading vehicles:', error);
        this.errorMessage = 'Failed to load vehicles';
      }
    });
  }

  loadOrders() {
    this.customerService.getOrders().subscribe({
      next: (data) => {
        console.log('Orders data:', data);
        console.log('Orders length:', data?.length);
        this.orders = data || [];
      },
      error: (error) => {
        console.error('Error loading orders:', error);
        this.errorMessage = 'Failed to load orders';
        this.orders = [];
      }
    });
  }

  loadWashPackages() {
    this.sharedService.getWashPackages().subscribe({
      next: (data) => {
        console.log('Wash packages data:', data);
        this.washPackages = data;
      },
      error: (error) => {
        console.error('Error loading wash packages:', error);
        // Use fallback data if API fails
        this.washPackages = [
          { id: 1, name: 'Basic Wash', price: 15.00, description: 'Exterior wash and dry' },
          { id: 2, name: 'Premium Wash', price: 25.00, description: 'Exterior + Interior cleaning' },
          { id: 3, name: 'Deluxe Wash', price: 35.00, description: 'Complete detailing service' }
        ];
      }
    });
  }

  loadAddons() {
    this.sharedService.getaddons().subscribe({
      next: (data) => {
        console.log('Addons data:', data);
        this.addons = data;
      },
      error: (error) => {
        console.error('Error loading addons:', error);
        this.addons = [];
      }
    });
  }

  loadRatings() {
    this.customerService.getRatings().subscribe({
      next: (ratings) => {
        console.log('Ratings data:', ratings);
        // Add rated order IDs to the set
        ratings.forEach(rating => {
          if (rating.orderId) {
            this.ratedOrders.add(rating.orderId);
          }
        });
      },
      error: (error) => {
        console.error('Error loading ratings:', error);
      }
    });
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  openAddVehicleModal() {
    this.showAddVehicleModal = true;
    this.newVehicle = { make: '', model: '', numberPlate: '', imageUrl: '' };
    this.addVehicleError = '';
  }

  closeAddVehicleModal() {
    this.showAddVehicleModal = false;
    this.addVehicleError = '';
  }

  addVehicle() {
    if (!this.newVehicle.make || !this.newVehicle.model || !this.newVehicle.numberPlate) {
      this.addVehicleError = 'Please fill in all required fields';
      return;
    }

    this.isLoading = true;
    this.addVehicleError = '';

    const vehicleData = {
      ...this.newVehicle,
      imageUrl: this.newVehicle.imageUrl || 'https://static.vecteezy.com/system/resources/previews/047/833/869/large_2x/green-bmw-sports-car-on-a-transparent-background-free-png.png'
    };

    this.customerService.addVehicle(vehicleData).subscribe({
      next: (response) => {
        console.log('Vehicle added successfully:', response);
        this.showSuccessToaster('Vehicle has been added successfully!');
        this.loadVehicles();
        this.closeAddVehicleModal();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error adding vehicle:', error);
        
        let errorMsg = 'Failed to add vehicle';
        if (error.error?.Details) {
          errorMsg = error.error.Details;
        } else if (error.error?.Message) {
          errorMsg = error.error.Message;
        } else if (error.error?.message) {
          errorMsg = error.error.message;
        } else if (error.message) {
          errorMsg = error.message;
        }
        
        this.addVehicleError = errorMsg;
        this.isLoading = false;
      }
    });
  }

  openPlaceOrderModal() {
    this.showPlaceOrderModal = true;
    this.newOrder = { vehicleId: '', washPackageId: '', orderDate: '', addonIds: [] as number[] };
    this.placeOrderError = '';
  }

  closePlaceOrderModal() {
    this.showPlaceOrderModal = false;
    this.placeOrderError = '';
  }

  placeOrder() {
    if (!this.newOrder.vehicleId || !this.newOrder.washPackageId || !this.newOrder.orderDate) {
      this.placeOrderError = 'Please fill in all required fields';
      return;
    }

    this.isLoading = true;
    this.placeOrderError = '';

    const orderData = {
      vehicleId: parseInt(this.newOrder.vehicleId),
      washPackageId: parseInt(this.newOrder.washPackageId),
      orderDate: new Date(this.newOrder.orderDate),
      addonIds: this.newOrder.addonIds.map(id => parseInt(id.toString()))
    };

    console.log('Placing order with data:', orderData);

    this.customerService.placeOrder(orderData).subscribe({
      next: (response) => {
        console.log('Order placed successfully:', response);
        this.showSuccessToaster('Order has been placed successfully!');
        this.loadOrders();
        this.newOrder = { vehicleId: '', washPackageId: '', orderDate: '', addonIds: [] as number[] };
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error placing order:', error);
        
        let errorMsg = 'Failed to place order';
        if (error.error?.Details) {
          errorMsg = error.error.Details;
        } else if (error.error?.Message) {
          errorMsg = error.error.Message;
        } else if (error.error?.message) {
          errorMsg = error.error.message;
        } else if (error.message) {
          errorMsg = error.message;
        }
        
        this.placeOrderError = errorMsg;
        this.isLoading = false;
      }
    });
  }

  toggleAddon(addonId: number) {
    const index = this.newOrder.addonIds.indexOf(addonId);
    if (index > -1) {
      this.newOrder.addonIds.splice(index, 1);
    } else {
      this.newOrder.addonIds.push(addonId);
    }
  }

  isAddonSelected(addonId: number): boolean {
    return this.newOrder.addonIds.includes(addonId);
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

  openPaymentModal(order: any) {
    console.log('Opening payment modal for order:', {
      id: order.id,
      status: order.status,
      paymentStatus: order.paymentStatus,
      totalAmount: order.totalAmount,
      vehicle: order.vehicle,
      washPackage: order.washPackage
    });
    
    this.selectedOrderForPayment = order;
    this.showPaymentModal = true;
    this.paymentError = ''; // Clear any previous errors
  }

  closePaymentModal() {
    this.showPaymentModal = false;
    this.selectedOrderForPayment = null;
    this.paymentError = ''; // Clear errors when closing
  }

  openReceiptModal(order: any) {
    this.customerService.getReceipt(order.id).subscribe({
      next: (receipt) => {
        console.log('Receipt data:', receipt);
        this.selectedReceipt = receipt;
        this.showReceiptModal = true;
      },
      error: (error) => {
        console.error('Error loading receipt:', error);
        console.error('Receipt error details:', error.error);
        // If API fails, use order data as fallback
        this.selectedReceipt = {
          orderId: order.id,
          serviceName: order.washPackage?.name || 'Car Wash Service',
          vehicleInfo: `${order.vehicle?.make || ''} ${order.vehicle?.model || ''}`.trim(),
          serviceDate: order.scheduledTime,
          totalAmount: order.totalAmount,
          paymentStatus: order.status
        };
        this.showReceiptModal = true;
      }
    });
  }

  closeReceiptModal() {
    this.showReceiptModal = false;
    this.selectedReceipt = null;
  }

  openRatingModal(order: any) {
    this.selectedOrderForRating = order;
    this.showRatingModal = true;
    this.rating = { rating: 5, comment: '' };
    this.ratingError = '';
  }

  closeRatingModal() {
    this.showRatingModal = false;
    this.selectedOrderForRating = null;
  }

  submitRating() {
    if (!this.selectedOrderForRating) return;

    if (!this.rating.rating || this.rating.rating < 1 || this.rating.rating > 5) {
      this.ratingError = 'Please select a rating between 1 and 5 stars';
      return;
    }

    this.isLoading = true;
    this.ratingError = '';

    // Backend expects specific format with lowercase property names
    const ratingData = {
      orderId: this.selectedOrderForRating.id,
      rating: this.rating.rating.toString(), // Convert to string
      review: this.rating.comment || 'string' // Use 'string' as default if empty
    };

    console.log('Order being rated:', this.selectedOrderForRating);
    console.log('Rating data being sent:', ratingData);
    console.log('Star count:', this.rating.rating);
    console.log('API endpoint:', 'http://localhost:5050/api/Customer/rating');

    this.customerService.submitRating(ratingData).subscribe({
      next: (response) => {
        console.log('Rating submitted successfully:', response);
        this.ratedOrders.add(this.selectedOrderForRating.id);
        this.showSuccessToaster('Rating submitted successfully!');
        this.loadOrders();
        this.closeRatingModal();
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Rating submission failed:', error);
        console.error('Error status:', error.status);
        console.error('Error details:', error.error);
        console.error('Request payload was:', ratingData);
        console.error('Full error response:', error.error);
        
        let errorMsg = 'Failed to submit rating';
        if (error.error?.Details) {
          errorMsg = error.error.Details;
          // If already rated, mark as rated and close modal
          if (error.error.Details === 'Order already rated') {
            this.ratedOrders.add(this.selectedOrderForRating.id);
            this.selectedOrderForRating.isRated = true;
            this.showSuccessToaster('Order already rated');
            this.closeRatingModal();
            this.isLoading = false;
            return;
          }
        } else if (error.error?.Message) {
          errorMsg = error.error.Message;
        } else if (error.error?.message) {
          errorMsg = error.error.message;
        } else if (error.message) {
          errorMsg = error.message;
        }
        
        this.ratingError = errorMsg;
        this.isLoading = false;
      }
    });
  }

  processRazorpayPayment() {
    if (!this.selectedOrderForPayment) return;

    // Check if payment already initiated
    if (this.selectedOrderForPayment.razorpayOrderId) {
      this.paymentError = 'Payment already initiated for this order';
      return;
    }

    // Check if order is already paid
    if (this.selectedOrderForPayment.paymentStatus === 'Paid') {
      this.paymentError = 'This order is already paid';
      return;
    }

    this.isLoading = true;
    this.paymentError = '';

    console.log('Processing payment for order:', {
      orderId: this.selectedOrderForPayment.id,
      amount: this.selectedOrderForPayment.totalAmount,
      status: this.selectedOrderForPayment.status,
      paymentStatus: this.selectedOrderForPayment.paymentStatus,
      razorpayOrderId: this.selectedOrderForPayment.razorpayOrderId
    });

    this.razorpayService.processPayment(this.selectedOrderForPayment.id, this.userProfile, Math.round(this.selectedOrderForPayment.totalAmount * 100) / 100)
      .then((response) => {
        console.log('Payment successful:', response);
        this.showSuccessToaster('Payment completed successfully!');
        this.loadOrders();
        this.closePaymentModal();
        this.isLoading = false;
      })
      .catch((error) => {
        console.error('Payment failed:', error);
        this.paymentError = error.message || 'Payment failed. Please try again.';
        this.isLoading = false;
      });
  }

  loadUserProfile() {
    this.customerService.getProfile().subscribe({
      next: (data) => {
        this.userProfile = data;
      },
      error: (error) => {
        console.error('Error loading profile:', error);
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

    console.log('Updating profile with data:', profileData);

    this.customerService.updateProfile(profileData).subscribe({
      next: (response) => {
        console.log('Profile update response:', response);
        this.showSuccessToaster('Profile updated successfully!');
        this.loadUserProfile(); // Reload profile data
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error updating profile:', error);
        console.error('Error details:', error.error);
        
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

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    window.location.href = '/login';
  }
}