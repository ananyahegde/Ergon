import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Notification } from '../models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);

  getAll() {
    return this.http.get<Notification[]>(`${environment.apiUrl}/notifications`);
  }

  markRead(notificationId: string) {
    return this.http.put<void>(`${environment.apiUrl}/notifications/${notificationId}/read`, {});
  }

  markAllRead() {
    return this.http.put<void>(`${environment.apiUrl}/notifications/read-all`, {});
  }

  delete(notificationId: string) {
    return this.http.delete<void>(`${environment.apiUrl}/notifications/${notificationId}`);
  }
}
