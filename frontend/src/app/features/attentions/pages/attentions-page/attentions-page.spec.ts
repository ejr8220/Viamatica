import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AttentionsPage } from './attentions-page';

describe('AttentionsPage', () => {
  let component: AttentionsPage;
  let fixture: ComponentFixture<AttentionsPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AttentionsPage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AttentionsPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
