import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './notifications.html',
  styleUrl: './notifications.css'
})
export class Notifications {
  notificationService = inject(NotificationService);
  notifications = this.notificationService.notifications;
  unreadCount = this.notificationService.unreadCount;
  isLoading = signal(false);

  markRead(notificationId: string) {
    const n = this.notifications().find(n => n.notificationId === notificationId);
    if (n?.isRead) return;
    this.notificationService.markRead(notificationId).subscribe();
  }

  markAllRead() {
    this.notificationService.markAllRead().subscribe();
  }

  delete(notificationId: string) {
    this.notificationService.delete(notificationId).subscribe();
  }
}
