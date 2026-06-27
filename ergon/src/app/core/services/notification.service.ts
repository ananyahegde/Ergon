import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { interval, Subscription } from 'rxjs';
import { startWith, switchMap, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { Notification } from '../models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);
  private _notifications = signal<Notification[]>([]);
  notifications = this._notifications.asReadonly();

  unreadCount = computed(() => this._notifications().filter(n => !n.isRead).length);

  private pollingSubscription?: Subscription;

  startPolling() {
    this.pollingSubscription = interval(30000).pipe(
      startWith(0),
      switchMap(() => this.http.get<Notification[]>(`${environment.apiUrl}/notifications`))
    ).subscribe(notifications => this._notifications.set(notifications));
  }

  stopPolling() {
    this.pollingSubscription?.unsubscribe();
  }

  markRead(notificationId: string) {
    return this.http.put<void>(`${environment.apiUrl}/notifications/${notificationId}/read`, {}).pipe(
      tap(() => this._notifications.update(list =>
        list.map(n => n.notificationId === notificationId ? { ...n, isRead: true } : n)
      ))
    );
  }

  markAllRead() {
    return this.http.put<void>(`${environment.apiUrl}/notifications/read-all`, {}).pipe(
      tap(() => this._notifications.update(list => list.map(n => ({ ...n, isRead: true }))))
    );
  }

  delete(notificationId: string) {
    return this.http.delete<void>(`${environment.apiUrl}/notifications/${notificationId}`).pipe(
      tap(() => this._notifications.update(list => list.filter(n => n.notificationId !== notificationId)))
    );
  }
}