import { Component, Input, OnInit, inject } from '@angular/core';
import { AnalysisResult, Trade } from '../../shared/shared.model';
import { BinanceService } from '../../core/services/binance.service';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-trade-analytics',
  standalone: true,
  imports: [
    MatTableModule,
    MatSortModule,
    CommonModule,
  ],
  templateUrl: './trade-analytics.component.html',
  styleUrl: './trade-analytics.component.css'
})
export class TradeAnalyticsComponent implements OnInit {
  private binanceService = inject(BinanceService);
  @Input() trades: Partial<Trade & { analytics: AnalysisResult }>[] = [];

  ngOnInit(): void {
    this.trades.forEach(trade => {
      this.analyze(trade);
    })
  }

  analyze(trade: Partial<Trade & { analytics: AnalysisResult }>): void {
    let price$ = this.binanceService.getCryptoPriceUpdates(trade.currency ?? '');
    price$.subscribe({
      next: data => {
        const currentPrice = parseFloat(data.c);
        if (trade.price) {
          const roi = ((trade.price - currentPrice) / trade.price) * 100;
          const recommendation = this.getRecommendation(roi);

          trade.analytics = {
            roi: roi,
            recommendation: recommendation,
          }
        }
      }
    });
  }

  getRecommendation(roi: number): string {
    let recommendation = '';
    if (roi > 50) {
      recommendation = 'Sell to lock in a profit.';
    } else if (roi > 10) {
      recommendation = 'Keep holding, further growth is possible.';
    } else if (roi < 0) {
      recommendation = 'Sell to minimize losses.';
    } else {
      recommendation = 'Keep it if you expect the price to recover.'; 
    }

    return recommendation;
  }
}
