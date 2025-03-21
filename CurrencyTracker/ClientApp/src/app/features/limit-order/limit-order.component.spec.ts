import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LimitOrderComponent } from './limit-order.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('LimitOrderComponent', () => {
  let component: LimitOrderComponent;
  let fixture: ComponentFixture<LimitOrderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LimitOrderComponent, BrowserAnimationsModule]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LimitOrderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
