import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-main-layout',
  standalone: false,
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss',
})
export class MainLayout implements OnInit {
  sidebarVisible = true;

  ngOnInit(): void {}

  toggleSidebar(): void {
    this.sidebarVisible = !this.sidebarVisible;
  }
}
