import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { UserService } from '../../../core/services/user.service';
import { AdminCreateUserRequest, UserRequest } from '../../../core/models/user.model';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.scss'
})
export class UserFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private userService = inject(UserService);

  isEditMode = false;
  userId: number | null = null;
  loading = false;
  loadingData = false;
  errorMessage = '';

  form = this.fb.group({
    name:     ['', Validators.required],
    role:     ['Customer', Validators.required],
    location: ['', Validators.required],
    // create-only fields
    email:    [''],
    password: ['']
  });

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEditMode = true;
      this.userId = Number(idParam);
      this.form.get('email')!.clearValidators();
      this.form.get('password')!.clearValidators();
      this.loadUser(this.userId);
    } else {
      this.form.get('email')!.setValidators([Validators.required, Validators.email]);
      this.form.get('password')!.setValidators([Validators.required, Validators.minLength(6)]);
    }
    this.form.get('email')!.updateValueAndValidity();
    this.form.get('password')!.updateValueAndValidity();
  }

  loadUser(id: number): void {
    this.loadingData = true;
    this.userService.getById(id).subscribe({
      next: (user) => {
        this.form.patchValue({
          name:     user.name,
          role:     user.role,
          location: user.location
        });
        this.loadingData = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load user data.';
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

    if (this.isEditMode) {
      const dto: UserRequest = {
        name:     this.form.value.name!,
        role:     this.form.value.role!,
        location: this.form.value.location!
      };
      this.userService.update(this.userId!, dto).subscribe({
        next: () => { this.loading = false; this.router.navigate(['/users']); },
        error: (err) => {
          this.loading = false;
          this.errorMessage = err?.error?.message || 'Failed to update user.';
        }
      });
    } else {
      const dto: AdminCreateUserRequest = {
        email:    this.form.value.email!,
        password: this.form.value.password!,
        name:     this.form.value.name!,
        role:     this.form.value.role!,
        location: this.form.value.location!
      };
      this.userService.create(dto).subscribe({
        next: () => { this.loading = false; this.router.navigate(['/users']); },
        error: (err) => {
          this.loading = false;
          this.errorMessage = err?.error?.message || 'Failed to create user.';
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/users']);
  }
}
