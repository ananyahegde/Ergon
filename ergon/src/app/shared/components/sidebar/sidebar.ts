import { Component, computed, inject } from '@angular/core';
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

  visibleNavItems = computed(() => {
    const role = this.authService.currentUser()?.role ?? '';
    return NAV_ITEMS.filter(item => item.roles.includes(role));
  });
}
