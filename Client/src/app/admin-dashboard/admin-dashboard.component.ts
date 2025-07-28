import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AdminService } from '../core/admin.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit {
  activeTab = 'dashboard';
  isLoading = false;
  successMessage = '';
  showToaster = false;

  // Dashboard stats
  dashboardStats = {
    totalCustomers: 6,
    totalWashers: 2,
    activeWashers: 2,
    totalWashPackages: 3,
    totalAddons: 4,
    orderStats: {
      totalOrders: 11,
      pendingOrders: 0,
      completedOrders: 1,
      totalRevenue: 24
    }
  };

  // Data arrays
  washPackages: any[] = [
    { id: 1, name: 'Basic Wash', price: 15.00, description: 'Exterior wash and dry', isActive: true },
    { id: 2, name: 'Premium Wash', price: 25.00, description: 'Exterior + Interior cleaning', isActive: true },
    { id: 3, name: 'Deluxe Wash', price: 35.00, description: 'Complete detailing service', isActive: true }
  ];

  addons: any[] = [
    { id: 1, name: 'Wax Polish', price: 10.00, description: 'Premium wax coating', isActive: true },
    { id: 2, name: 'Engine Cleaning', price: 15.00, description: 'Engine bay cleaning', isActive: true },
    { id: 3, name: 'Tire Shine', price: 8.00, description: 'Tire cleaning and shine', isActive: true },
    { id: 4, name: 'Interior Vacuum', price: 12.00, description: 'Deep interior cleaning', isActive: true }
  ];

  washers: any[] = [
    { id: 1, name: 'John Washer', email: 'john@example.com', phone: '+1234567890', isActive: true, ordersCompleted: 25 },
    { id: 2, name: 'Mike Cleaner', email: 'mike@example.com', phone: '+0987654321', isActive: true, ordersCompleted: 18 }
  ];

  customers: any[] = [
    { id: 1, name: 'Alice Johnson', email: 'alice@example.com', phone: '+1111111111', isActive: true, totalOrders: 5 },
    { id: 2, name: 'Bob Smith', email: 'bob@example.com', phone: '+2222222222', isActive: true, totalOrders: 3 },
    { id: 3, name: 'Charlie Brown', email: 'charlie@example.com', phone: '+3333333333', isActive: false, totalOrders: 2 }
  ];

  orders: any[] = [];

  // Modal states
  showPackageModal = false;
  showAddonModal = false;
  selectedPackage: any = null;
  selectedAddon: any = null;
  newPackage = { name: '', price: 0, description: '' };
  newAddon = { name: '', price: 0, description: '' };

  constructor(private adminService: AdminService) {}

  ngOnInit() {
    this.loadDashboardStats();
    this.loadWashPackages();
    this.loadWashers();
    this.loadOrders();
    this.loadCustomers();
  }

  loadDashboardStats() {
    console.log('Token in localStorage:', localStorage.getItem('token'));
    console.log('User in localStorage:', localStorage.getItem('user'));
    
    this.adminService.getDashboardStats().subscribe({
      next: (data) => {
        this.dashboardStats = data;
      },
      error: (error) => {
        console.error('Error loading dashboard stats:', error);
        console.error('Error status:', error.status);
        console.error('Error details:', error.error);
        
        if (error.status === 403) {
          console.error('403 Forbidden - Check if user has Admin role');
        }
        // Using fallback data
      }
    });
  }

  loadWashPackages() {
    this.adminService.getWashPackages().subscribe({
      next: (data) => {
        this.washPackages = data || [];
      },
      error: (error) => {
        console.error('Error loading wash packages:', error);
        // Using fallback data
      }
    });
  }

  loadWashers() {
    this.adminService.getWashers().subscribe({
      next: (data) => {
        this.washers = data || [];
      },
      error: (error) => {
        console.error('Error loading washers:', error);
        // Using fallback data
      }
    });
  }

  loadOrders() {
    this.adminService.getAllOrders().subscribe({
      next: (data) => {
        this.orders = data || [];
      },
      error: (error) => {
        console.error('Error loading orders:', error);
        // Using fallback data
      }
    });
  }

  loadCustomers() {
    this.adminService.getCustomers().subscribe({
      next: (data) => {
        this.customers = data || [];
      },
      error: (error) => {
        console.error('Error loading customers:', error);
        // Using fallback data
      }
    });
  }

  setActiveTab(tab: string) {
    this.activeTab = tab;
  }

  // Wash Package Methods
  openPackageModal(packageItem?: any) {
    this.selectedPackage = packageItem;
    this.newPackage = packageItem ? { ...packageItem } : { name: '', price: 0, description: '' };
    this.showPackageModal = true;
  }

  closePackageModal() {
    this.showPackageModal = false;
    this.selectedPackage = null;
  }

  savePackage() {
    if (this.selectedPackage) {
      // Update existing
      this.adminService.updateWashPackage(this.selectedPackage.id, this.newPackage).subscribe({
        next: (response) => {
          this.showSuccessToaster('Package updated successfully!');
          this.loadWashPackages();
          this.closePackageModal();
        },
        error: (error) => {
          console.error('Error updating package:', error);
          this.showSuccessToaster('Failed to update package');
        }
      });
    } else {
      // Create new
      this.adminService.createWashPackage(this.newPackage).subscribe({
        next: (response) => {
          this.showSuccessToaster('Package created successfully!');
          this.loadWashPackages();
          this.closePackageModal();
        },
        error: (error) => {
          console.error('Error creating package:', error);
          this.showSuccessToaster('Failed to create package');
        }
      });
    }
  }

  deletePackage(packageItem: any) {
    this.adminService.deleteWashPackage(packageItem.id).subscribe({
      next: (response) => {
        this.showSuccessToaster('Package deleted successfully!');
        this.loadWashPackages();
      },
      error: (error) => {
        console.error('Error deleting package:', error);
        this.showSuccessToaster('Failed to delete package');
      }
    });
  }

  // Addon Methods
  openAddonModal(addonItem?: any) {
    this.selectedAddon = addonItem;
    this.newAddon = addonItem ? { ...addonItem } : { name: '', price: 0, description: '' };
    this.showAddonModal = true;
  }

  closeAddonModal() {
    this.showAddonModal = false;
    this.selectedAddon = null;
  }

  saveAddon() {
    if (this.selectedAddon) {
      // Update existing
      const index = this.addons.findIndex(a => a.id === this.selectedAddon.id);
      if (index !== -1) {
        this.addons[index] = { ...this.newAddon, id: this.selectedAddon.id, isActive: true };
      }
      this.showSuccessToaster('Addon updated successfully!');
    } else {
      // Create new
      const newId = Math.max(...this.addons.map(a => a.id)) + 1;
      this.addons.push({ ...this.newAddon, id: newId, isActive: true });
      this.showSuccessToaster('Addon created successfully!');
    }
    this.closeAddonModal();
  }

  deleteAddon(addonItem: any) {
    this.addons = this.addons.filter(a => a.id !== addonItem.id);
    this.showSuccessToaster('Addon deleted successfully!');
  }

  // Washer Methods
  toggleWasherStatus(washer: any) {
    const newStatus = !washer.isActive;
    const statusData = { isActive: newStatus };
    
    console.log('Updating washer status:', {
      washerId: washer.id,
      currentStatus: washer.isActive,
      newStatus: newStatus,
      statusData: statusData
    });
    
    this.adminService.updateWasherStatus(washer.id, statusData).subscribe({
      next: (response) => {
        console.log('Washer status update response:', response);
        washer.isActive = newStatus;
        const status = newStatus ? 'activated' : 'deactivated';
        this.showSuccessToaster(`Washer ${status} successfully!`);
        this.loadWashers();
      },
      error: (error) => {
        console.error('Error updating washer status:', error);
        console.error('Error status:', error.status);
        console.error('Error details:', error.error);
        
        let errorMsg = 'Failed to update washer status';
        if (error.error?.Details) {
          errorMsg = error.error.Details;
        } else if (error.error?.Message) {
          errorMsg = error.error.Message;
        } else if (error.error?.message) {
          errorMsg = error.error.message;
        } else if (error.message) {
          errorMsg = error.message;
        }
        
        this.showSuccessToaster(errorMsg);
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