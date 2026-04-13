import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { UserService } from '../../../core/services/user.service';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent implements OnInit {
  private userService = inject(UserService);
  private router = inject(Router);

  users: User[] = [];
  loading = false;
  errorMessage = '';
  actionMessage = '';

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.errorMessage = '';
    this.userService.getAll().subscribe({
      next: (users) => {
        this.users = users;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Failed to load users. Please try again.';
        this.loading = false;
      }
    });
  }

  goToEdit(id: number): void {
    this.router.navigate(['/users', id, 'edit']);
  }

  deleteUser(user: User, event: Event): void {
    event.stopPropagation();
    if (!confirm(`Delete user "${user.name}"? This cannot be undone.`)) return;

    this.userService.delete(user.id).subscribe({
      next: () => {
        this.actionMessage = `User "${user.name}" deleted.`;
        this.loadUsers();
        setTimeout(() => this.actionMessage = '', 3000);
      },
      error: () => {
        this.errorMessage = 'Failed to delete user.';
        setTimeout(() => this.errorMessage = '', 3000);
      }
    });
  }
}
