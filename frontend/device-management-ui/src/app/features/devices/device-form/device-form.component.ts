import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { DeviceService } from '../../../core/services/device.service';
import { DeviceRequest } from '../../../core/models/device.model';

@Component({
  selector: 'app-device-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './device-form.component.html',
  styleUrl: './device-form.component.scss'
})
export class DeviceFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private deviceService = inject(DeviceService);

  isEditMode = false;
  deviceId: number | null = null;
  loading = false;
  loadingData = false;
  generatingDescription = false;
  errorMessage = '';

  form = this.fb.group({
    name: ['', Validators.required],
    manufacturer: ['', Validators.required],
    type: [0, Validators.required],
    operatingSystem: ['', Validators.required],
    osVersion: ['', Validators.required],
    processor: ['', Validators.required],
    ram: [null as number | null, [Validators.required, Validators.min(1)]],
    description: ['']
  });

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEditMode = true;
      this.deviceId = Number(idParam);
      this.loadDevice(this.deviceId);
    }
  }

  loadDevice(id: number): void {
    this.loadingData = true;
    this.deviceService.getById(id).subscribe({
      next: (device) => {
        const typeValue = device.type === 'Phone' ? 0 : device.type === 'Tablet' ? 1 : 0;
        this.form.patchValue({
          name: device.name,
          manufacturer: device.manufacturer,
          type: typeValue,
          operatingSystem: device.operatingSystem,
          osVersion: device.osVersion,
          processor: device.processor,
          ram: device.ram,
          description: device.description || ''
        });
        this.loadingData = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load device data.';
        this.loadingData = false;
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    this.errorMessage = '';

    const dto: DeviceRequest = {
      name: this.form.value.name!,
      manufacturer: this.form.value.manufacturer!,
      type: Number(this.form.value.type),
      operatingSystem: this.form.value.operatingSystem!,
      osVersion: this.form.value.osVersion!,
      processor: this.form.value.processor!,
      ram: Number(this.form.value.ram),
      description: this.form.value.description || undefined
    };

    const request$ = this.isEditMode
      ? this.deviceService.update(this.deviceId!, dto)
      : this.deviceService.create(dto);

    request$.subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/devices']);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.error?.message || err?.error || 'Failed to save device. Please try again.';
      }
    });
  }

  generateDescription(): void {
    const v = this.form.value;
    if (!v.name || !v.manufacturer || !v.operatingSystem || !v.processor || !v.ram) {
      this.errorMessage = 'Please fill in the required fields before generating a description.';
      return;
    }

    this.generatingDescription = true;
    this.errorMessage = '';

    const dto: DeviceRequest = {
      name: v.name!,
      manufacturer: v.manufacturer!,
      type: Number(v.type),
      operatingSystem: v.operatingSystem!,
      osVersion: v.osVersion || '',
      processor: v.processor!,
      ram: Number(v.ram),
    };

    this.deviceService.generateDescription(dto).subscribe({
      next: (res) => {
        this.form.patchValue({ description: res.description });
        this.generatingDescription = false;
      },
      error: (err) => {
        this.generatingDescription = false;
        this.errorMessage = err?.error?.message || 'Failed to generate description. Make sure Ollama is running.';
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/devices']);
  }
}
