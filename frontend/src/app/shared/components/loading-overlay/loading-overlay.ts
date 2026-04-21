import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { LoadingService } from '../../../core/services';

@Component({
  selector: 'app-loading-overlay',
  standalone: false,
  templateUrl: './loading-overlay.html',
  styleUrl: './loading-overlay.scss',
})
export class LoadingOverlay implements OnInit, OnDestroy {
  loading = false;
  private subscription?: Subscription;

  constructor(private loadingService: LoadingService) {}

  ngOnInit(): void {
    this.subscription = this.loadingService.loading$.subscribe(
      (loading) => (this.loading = loading)
    );
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
}
