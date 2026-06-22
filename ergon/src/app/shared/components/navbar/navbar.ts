import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar {
  private authService = inject(AuthService);

  currentUser = this.authService.currentUser;
  showDropdown = signal(false);

  toggleDropdown() {
    this.showDropdown.update(v => !v);
  }

  logout() {
    this.authService.logout();
  }
}
