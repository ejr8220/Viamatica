import { Component, OnInit } from '@angular/core';

import { DashboardService, DashboardStat } from '../../services/dashboard.service';

@Component({
  selector: 'app-dashboard-page',
  standalone: false,
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.scss',
})
export class DashboardPage implements OnInit {
  stats: DashboardStat[] = [];
  loading = false;

  constructor(private readonly dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loading = true;
    this.dashboardService.loadStats().subscribe({
      next: (stats) => {
        this.stats = stats;
      },
      complete: () => {
        this.loading = false;
      },
    });
  }
}
