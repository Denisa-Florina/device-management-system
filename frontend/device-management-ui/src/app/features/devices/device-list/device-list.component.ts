import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { DeviceService } from '../../../core/services/device.service';
import { AuthService } from '../../../core/services/auth.service';
import { Device } from '../../../core/models/device.model';

@Component({
  selector: 'app-device-list',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './device-list.component.html',
  styleUrl: './device-list.component.scss'
})
export class DeviceListComponent implements OnInit, OnDestroy {
  private deviceService = inject(DeviceService);
  authService = inject(AuthService);
  private router = inject(Router);

  devices: Device[] = [];
  loading = false;
  refreshing = false;
  errorMessage = '';
  actionMessage = '';
  searchQuery = '';

  private searchSubject = new Subject<string>();
  private searchSub?: Subscription;

  ngOnInit(): void {
    this.loadDevices(true);

    this.searchSub = this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(query => {
        this.refreshing = true;
        this.errorMessage = '';
        if (!query.trim()) {
          return this.authService.isAdmin()
            ? this.deviceService.getAll()
            : this.deviceService.getMyView();
        }
        return this.deviceService.search(query.trim());
      })
    ).subscribe({
      next: (devices) => {
        this.devices = devices;
        this.loading = false;
        this.refreshing = false;
      },
      error: () => {
        this.errorMessage = 'Search failed. Please try again.';
        this.loading = false;
        this.refreshing = false;
      }
    });
  }

  ngOnDestroy(): void {
    this.searchSub?.unsubscribe();
  }

  onSearchChange(query: string): void {
    this.searchSubject.next(query);
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.searchSubject.next('');
  }

  loadDevices(showSpinner = false): void {
    if (showSpinner) {
      this.loading = true;
    }
    this.refreshing = true;
    this.errorMessage = '';

    const request$ = this.authService.isAdmin()
      ? this.deviceService.getAll()
      : this.deviceService.getMyView();

    request$.subscribe({
      next: (devices) => {
        this.devices = devices;
        this.loading = false;
        this.refreshing = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load devices. Please try again.';
        this.loading = false;
        this.refreshing = false;
      }
    });
  }

  isMyDevice(device: Device): boolean {
    return device.currentUserId === this.authService.getCurrentUserId();
  }

  goToDetail(id: number): void {
    this.router.navigate(['/devices', id]);
  }

  deleteDevice(id: number, event: Event): void {
    event.stopPropagation();
    if (!confirm('Are you sure you want to delete this device? This action cannot be undone.')) {
      return;
    }
    this.deviceService.delete(id).subscribe({
      next: () => {
        this.actionMessage = 'Device deleted successfully.';
        this.loadDevices();
        setTimeout(() => this.actionMessage = '', 3000);
      },
      error: () => {
        this.errorMessage = 'Failed to delete device.';
        setTimeout(() => this.errorMessage = '', 3000);
      }
    });
  }

  assignDevice(device: Device, event: Event): void {
    event.stopPropagation();
    const location = prompt('Enter the location for this device:');
    if (location === null) return;
    if (!location.trim()) {
      alert('Location cannot be empty.');
      return;
    }
    this.deviceService.selfAssign({ deviceId: device.id, location: location.trim() }).subscribe({
      next: () => {
        this.actionMessage = `Device "${device.name}" assigned successfully.`;
        this.loadDevices();
        setTimeout(() => this.actionMessage = '', 3000);
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Failed to assign device.';
        this.refreshing = false;
        setTimeout(() => this.errorMessage = '', 3000);
      }
    });
  }

  unassignDevice(device: Device, event: Event): void {
    event.stopPropagation();
    if (!confirm(`Unassign device "${device.name}"?`)) return;
    this.deviceService.selfUnassign().subscribe({
      next: () => {
        this.actionMessage = `Device "${device.name}" unassigned successfully.`;
        this.loadDevices();
        setTimeout(() => this.actionMessage = '', 3000);
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Failed to unassign device.';
        this.refreshing = false;
        setTimeout(() => this.errorMessage = '', 3000);
      }
    });
  }
}
