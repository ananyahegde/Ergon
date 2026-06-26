import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MasterService } from './core/services/master.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('ergon');
  private masterService = inject(MasterService);

  ngOnInit() {
    this.masterService.loadAll().subscribe();
  }
}