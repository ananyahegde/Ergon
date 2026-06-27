import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { NAV_ITEMS } from '../../config/nav.config';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.css'
})
export class Sidebar {
  private authService = inject(AuthService);

  isOpen = signal(false);
  open() { this.isOpen.set(true); }
  close() { this.isOpen.set(false); }
  closeOnMobile() { if (window.innerWidth < 768) this.close(); }

  visibleNavItems = computed(() => {
    const role = this.authService.currentUser()?.role ?? '';
    return NAV_ITEMS.filter(item => item.roles.includes(role));
  });
}
