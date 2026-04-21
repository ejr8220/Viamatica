import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-stats-section',
  standalone: false,
  templateUrl: './stats-section.html',
  styleUrl: './stats-section.scss',
})
export class StatsSection {
  @Input() title = 'Indicadores';
  @Input() collapsed = true;

  toggle(): void {
    this.collapsed = !this.collapsed;
  }
}
