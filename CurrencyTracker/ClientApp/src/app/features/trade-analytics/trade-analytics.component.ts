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
  readonly displayedColumns = Constants.ANALYSIS_COLUMNS;
  private binanceService = inject(BinanceService);
  private tradeService = inject(TradesService);
  dataSource = new MatTableDataSource<AnalysisResult>();

  ngOnInit(): void {
    this.tradeService.analyzedTrades$.subscribe(data => {
      this.dataSource.data = data.map((trade, index) => ({
        ...trade,
        position: index + 1,
      }));
    })
  }
}
