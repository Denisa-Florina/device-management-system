import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User, UserRequest, AdminCreateUserRequest } from '../models/user.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class UserService {
  private http = inject(HttpClient);

  getAll(): Observable<User[]> {
    return this.http.get<User[]>(`${environment.apiUrl}/api/users`);
  }

  getById(id: number): Observable<User> {
    return this.http.get<User>(`${environment.apiUrl}/api/users/${id}`);
  }

  create(dto: AdminCreateUserRequest): Observable<User> {
    return this.http.post<User>(`${environment.apiUrl}/api/users`, dto);
  }

  update(id: number, dto: UserRequest): Observable<User> {
    return this.http.put<User>(`${environment.apiUrl}/api/users/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/api/users/${id}`);
  }
}
