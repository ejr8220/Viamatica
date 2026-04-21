export interface MenuItem {
  navigationMenuId: number;
  menuKey: string;
  label: string;
  icon: string;
  route: string | null;
  displayOrder: number;
  children?: MenuItem[];
}
