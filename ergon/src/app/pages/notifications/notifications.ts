import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { NotificationService } from '../../core/services/notification.service';
import { Notification } from '../../core/models/notification.model';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './notifications.html',
  styleUrl: './notifications.css'
})
export class Notifications implements OnInit {
  private notificationService = inject(NotificationService);

  notifications = signal<Notification[]>([]);
  isLoading = signal(true);

  unreadCount = computed(() => this.notifications().filter(n => !n.isRead).length);

  ngOnInit() {
    this.notificationService.getAll().subscribe({
      next: (data) => {
        this.notifications.set(data);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  markRead(notificationId: string) {
    if (this.notifications().find(n => n.notificationId === notificationId)?.isRead) return;
    this.notificationService.markRead(notificationId).subscribe({
      next: () => {
        this.notifications.update(list =>
          list.map(n => n.notificationId === notificationId ? { ...n, isRead: true } : n)
        );
      },
      error: () => {}
    });
  }

  markAllRead() {
    this.notificationService.markAllRead().subscribe({
      next: () => {
        this.notifications.update(list => list.map(n => ({ ...n, isRead: true })));
      },
      error: () => {}
    });
  }

  delete(notificationId: string) {
    this.notificationService.delete(notificationId).subscribe({
      next: () => {
        this.notifications.update(list => list.filter(n => n.notificationId !== notificationId));
      },
      error: () => {}
    });
  }
}