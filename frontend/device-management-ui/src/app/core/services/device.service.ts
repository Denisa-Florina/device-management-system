import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Device, DeviceRequest } from '../models/device.model';
import { SelfAssignRequest } from '../models/auth.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class DeviceService {
  private http = inject(HttpClient);

  getAll(): Observable<Device[]> {
    return this.http.get<Device[]>(`${environment.apiUrl}/api/devices`);
  }

  getMyView(): Observable<Device[]> {
    return this.http.get<Device[]>(`${environment.apiUrl}/api/devices/my-view`);
  }

  getById(id: number): Observable<Device> {
    return this.http.get<Device>(`${environment.apiUrl}/api/devices/${id}`);
  }

  create(dto: DeviceRequest): Observable<Device> {
    return this.http.post<Device>(`${environment.apiUrl}/api/devices`, dto);
  }

  update(id: number, dto: DeviceRequest): Observable<Device> {
    return this.http.put<Device>(`${environment.apiUrl}/api/devices/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/api/devices/${id}`);
  }

  generateDescription(dto: DeviceRequest): Observable<{ description: string }> {
    return this.http.post<{ description: string }>(`${environment.apiUrl}/api/devices/generate-description`, dto);
  }

  search(query: string): Observable<Device[]> {
    return this.http.get<Device[]>(`${environment.apiUrl}/api/devices/search`, {
      params: { q: query }
    });
  }

  selfAssign(dto: SelfAssignRequest): Observable<any> {
    return this.http.post(`${environment.apiUrl}/api/deviceassignments/self-assign`, dto);
  }

  selfUnassign(): Observable<any> {
    return this.http.delete(`${environment.apiUrl}/api/deviceassignments/self-unassign`);
  }
}
