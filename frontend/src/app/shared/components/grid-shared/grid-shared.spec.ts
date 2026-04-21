import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GridShared } from './grid-shared';

describe('GridShared', () => {
  let component: GridShared;
  let fixture: ComponentFixture<GridShared>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GridShared]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GridShared);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
