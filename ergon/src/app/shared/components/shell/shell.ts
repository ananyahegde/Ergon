import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Navbar } from '../navbar/navbar';
import { Sidebar } from '../sidebar/sidebar';
import { Toast } from '../toast/toast';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, Navbar, Sidebar, Toast],
  templateUrl: './shell.html',
  styleUrl: './shell.css'
})
export class Shell {}
