import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-stat-card',
  standalone: false,
  templateUrl: './stat-card.html',
  styleUrl: './stat-card.scss',
})
export class StatCard {
  @Input() title: string = '';
  @Input() value: string | number = '';
  @Input() icon: string = 'pi pi-chart-line';
  @Input() color: string = 'primary';
  @Input() trend?: number;
}
