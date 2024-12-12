import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TradeAnalyticsComponent } from './trade-analytics.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('TradeAnalyticsComponent', () => {
  let component: TradeAnalyticsComponent;
  let fixture: ComponentFixture<TradeAnalyticsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TradeAnalyticsComponent, BrowserAnimationsModule],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting() 
     ]
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
