import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { EmployeeService } from '../../../core/services/employee.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css'
})
export class Navbar implements OnInit {
  private authService = inject(AuthService);
  private employeeService = inject(EmployeeService);

  currentUser = this.authService.currentUser;
  showDropdown = signal(false);
  avatarUrl = signal<string | null>(null);

  ngOnInit() {
    const id = this.currentUser()?.id;
    if (id) {
      this.employeeService.getPfp(id).subscribe({
        next: (blob) => this.avatarUrl.set(URL.createObjectURL(blob)),
        error: () => {}
      });
    }
  }

  getInitials(): string {
    const user = this.currentUser();
    if (!user) return '';
    return user.firstName.charAt(0).toUpperCase();
  }

  toggleDropdown() {
    this.showDropdown.update(v => !v);
  }

  logout() {
    this.authService.logout();
  }
}