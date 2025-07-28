import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-loading-3d',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loading-3d.component.html',
  styleUrls: ['./loading-3d.component.scss']
})
export class Loading3dComponent {
  @Input() status: 'pending' | 'accepted' | 'started' | 'completed' = 'pending';
  @Input() isVisible: boolean = false;

  getStatusText(): string {
    switch (this.status) {
      case 'pending': return 'Order Pending...';
      case 'accepted': return 'Order Accepted...';
      case 'started': return 'Wash Started...';
      case 'completed': return 'Order Completed!';
      default: return 'Processing...';
    }
  }

  getStatusColor(): string {
    switch (this.status) {
      case 'pending': return 'text-yellow-400';
      case 'accepted': return 'text-blue-400';
      case 'started': return 'text-green-400';
      case 'completed': return 'text-green-500';
      default: return 'text-gray-400';
    }
  }
}