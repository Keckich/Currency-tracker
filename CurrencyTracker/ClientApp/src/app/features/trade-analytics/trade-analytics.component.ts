import { Component, Input, OnInit, inject } from '@angular/core';
import { AnalysisResult, Trade } from '../../shared/shared.model';
import { BinanceService } from '../../core/services/binance.service';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { CommonModule } from '@angular/common';
import { TradesService } from '../../core/services/trades.service';
import { Constants } from '../../shared/constants.value';

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
  private tradeService = inject(TradesService);
  trades: Partial<Trade & { analytics: AnalysisResult }>[] = [];
  dataSource = new MatTableDataSource<Partial<Trade>>();
  displayedColumns = ['position', 'currency', 'avgPrice', 'totalAmount', 'roi', 'recommendation'];

  ngOnInit(): void {
    this.tradeService.analyzedTrades$.subscribe(data => {
      this.dataSource.data = data.map((trade, index) => ({
        ...trade,
        position: index + 1,
        recommendation: this.getRecommendation(trade.roi)
      }));
    })
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
