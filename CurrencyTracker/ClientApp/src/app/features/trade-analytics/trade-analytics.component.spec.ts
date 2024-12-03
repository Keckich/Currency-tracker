import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TradeAnalyticsComponent } from './trade-analytics.component';

describe('TradeAnalyticsComponent', () => {
  let component: TradeAnalyticsComponent;
  let fixture: ComponentFixture<TradeAnalyticsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TradeAnalyticsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TradeAnalyticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
