import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CashesPage } from './cashes-page';

describe('CashesPage', () => {
  let component: CashesPage;
  let fixture: ComponentFixture<CashesPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CashesPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CashesPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
