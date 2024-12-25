import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TradeAnalyticsComponent } from './trade-analytics.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatTableDataSource } from '@angular/material/table';
import { By } from '@angular/platform-browser';
import { AnalysisResult } from '../../shared/shared.model';
import { AnalysisRecommenations } from '../../shared/constants.value';

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

  it('should display analyzed trades in table', () => {
    const analyzedTrades: AnalysisResult[] = [
      { avgPrice: 90000, currency: 'BTC', roi: 52, recommendation: AnalysisRecommenations.SELL_PROFIT, tradeInfo: { totalAmount: 2, totalSpent: 135000 } },
      { avgPrice: 3500, currency: 'ETH', roi: 5, recommendation: AnalysisRecommenations.KEEP_RECOVER, tradeInfo: { totalAmount: 2, totalSpent: 6800 } }
    ]
    component.dataSource = new MatTableDataSource(analyzedTrades);
    fixture.detectChanges();

    const rows = fixture.debugElement.queryAll(By.css('.mat-mdc-row'));
    expect(rows.length).toBe(analyzedTrades.length);

    const firstRow = rows[0].nativeElement;
    expect(firstRow.textContent).toContain(analyzedTrades[0].recommendation);
    expect(firstRow.textContent).toContain(analyzedTrades[0].currency);
  });
});
