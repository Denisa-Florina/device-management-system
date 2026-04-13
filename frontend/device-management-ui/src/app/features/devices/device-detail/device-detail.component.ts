import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DeviceService } from '../../../core/services/device.service';
import { AuthService } from '../../../core/services/auth.service';
import { Device } from '../../../core/models/device.model';

@Component({
  selector: 'app-device-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './device-detail.component.html',
  styleUrl: './device-detail.component.scss'
})
export class DeviceDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private deviceService = inject(DeviceService);
  authService = inject(AuthService);

  device: Device | null = null;
  loading = false;
  errorMessage = '';
  actionMessage = '';

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDevice(id);
  }

  loadDevice(id: number): void {
    this.loading = true;
    this.deviceService.getById(id).subscribe({
      next: (device) => {
        this.device = device;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Device not found or failed to load.';
        this.loading = false;
      }
    });
  }

  deleteDevice(): void {
    if (!this.device) return;
    if (!confirm(`Are you sure you want to delete "${this.device.name}"? This action cannot be undone.`)) return;

    this.deviceService.delete(this.device.id).subscribe({
      next: () => {
        this.router.navigate(['/devices']);
      },
      error: () => {
        this.errorMessage = 'Failed to delete device.';
      }
    });
  }

  assignDevice(): void {
    if (!this.device) return;
    const location = prompt('Enter the location for this device:');
    if (location === null) return;
    if (!location.trim()) {
      alert('Location cannot be empty.');
      return;
    }
    this.deviceService.selfAssign({ deviceId: this.device.id, location: location.trim() }).subscribe({
      next: () => {
        this.actionMessage = 'Device assigned successfully.';
        this.loadDevice(this.device!.id);
        setTimeout(() => this.actionMessage = '', 3000);
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Failed to assign device.';
        setTimeout(() => this.errorMessage = '', 3000);
      }
    });
  }

  unassignDevice(): void {
    if (!this.device) return;
    if (!confirm(`Unassign device "${this.device.name}"?`)) return;

    this.deviceService.selfUnassign().subscribe({
      next: () => {
        this.actionMessage = 'Device unassigned successfully.';
        this.loadDevice(this.device!.id);
        setTimeout(() => this.actionMessage = '', 3000);
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Failed to unassign device.';
        setTimeout(() => this.errorMessage = '', 3000);
      }
    });
  }

  isMyDevice(): boolean {
    return this.device?.currentUserId === this.authService.getCurrentUserId();
  }

  goBack(): void {
    this.router.navigate(['/devices']);
  }
}
