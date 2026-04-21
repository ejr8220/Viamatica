import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { MenuItem } from '../../core/models';
import { MenuService } from '../../core/services';

@Component({
  selector: 'app-sidebar',
  standalone: false,
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar implements OnInit, OnDestroy {
  @Input() visible = true;

  menuItems: MenuItem[] = [];
  expandedKeys: Record<string, boolean> = {};
  private subscription?: Subscription;

  constructor(
    private menuService: MenuService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.menuService.loadMenu().subscribe();
    this.subscription = this.menuService.menuItems$.subscribe((items) => {
      this.menuItems = items;
      this.syncExpandedState(items);
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  navigate(route: string | null, event?: Event): void {
    event?.preventDefault();
    event?.stopPropagation();

    if (route) {
      this.router.navigateByUrl(route);
    }
  }

  hasChildren(item: MenuItem): boolean {
    return !!item.children?.length;
  }

  isExpanded(item: MenuItem): boolean {
    return this.expandedKeys[item.menuKey] ?? true;
  }

  toggleGroup(item: MenuItem, event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.expandedKeys[item.menuKey] = !this.isExpanded(item);
  }

  isActive(route: string | null): boolean {
    return !!route && this.router.url.startsWith(route);
  }

  private syncExpandedState(items: MenuItem[]): void {
    const nextState: Record<string, boolean> = {};

    const visit = (nodes: MenuItem[]): void => {
      for (const node of nodes) {
        if (this.hasChildren(node)) {
          nextState[node.menuKey] = this.expandedKeys[node.menuKey] ?? true;
          visit(node.children ?? []);
        }
      }
    };

    visit(items);
    this.expandedKeys = nextState;
  }
}
