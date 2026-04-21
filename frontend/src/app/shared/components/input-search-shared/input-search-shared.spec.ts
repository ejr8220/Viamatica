import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InputSearchShared } from './input-search-shared';

describe('InputSearchShared', () => {
  let component: InputSearchShared;
  let fixture: ComponentFixture<InputSearchShared>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InputSearchShared]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InputSearchShared);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
