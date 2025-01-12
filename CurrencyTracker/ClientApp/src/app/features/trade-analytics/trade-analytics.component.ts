import { Component, Input, OnInit, ViewChild, inject } from '@angular/core';
import { AnalysisResult, Trade } from '../../shared/shared.model';
import { BinanceService } from '../../core/services/binance.service';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { CommonModule } from '@angular/common';
import { TradesService } from '../../core/services/trades.service';
import { Constants, PnLChartOptions, PnLIntervals } from '../../shared/constants.value';
import { ApexAxisChartSeries, NgApexchartsModule } from 'ng-apexcharts';
import { Subscription, map } from 'rxjs';
import { NgxSpinnerService } from 'ngx-spinner';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { PnLInterval } from '../../shared/shared.enum';
import { IntervalListComponent } from '../interval-list/interval-list.component';
import { ChartComponent } from 'ng-apexcharts';

@Component({
  selector: 'app-trade-analytics',
  standalone: true,
  imports: [
    NgApexchartsModule,
    MatTableModule,
    MatSortModule,
    CommonModule,
    LoadingSpinnerComponent,
    IntervalListComponent,
  ],
  templateUrl: './trade-analytics.component.html',
  styleUrl: './trade-analytics.component.css'
})
export class TradeAnalyticsComponent implements OnInit {
  readonly displayedColumns = Constants.ANALYSIS_COLUMNS;
  private binanceService = inject(BinanceService);
  private tradeService = inject(TradesService);
  private spinner = inject(NgxSpinnerService);

  isLoading: boolean = true;
  pnlIntervals: Partial<Record<PnLInterval, string>> = PnLIntervals;
  selectedInterval: PnLInterval = PnLInterval.D7;
  dataSource = new MatTableDataSource<AnalysisResult>();

  chartSeries: ApexAxisChartSeries = [
    {
      name: 'PnL',
      data: []
    }
  ]

  chartOptions = PnLChartOptions;

  ngOnInit(): void {
    this.spinner.show();
    this.tradeService.analyzedTrades$.subscribe(data => {
      this.dataSource.data = data.map((trade, index) => ({
        ...trade,
        position: index + 1,
      }));
    })

    this.reloadChart();
  }

  hideSpinner(): void {
    if (this.isLoading) {
      this.isLoading = false;
      this.spinner.hide();
    }
  }

  onIntervalChange(interval: number) {
    this.selectedInterval = interval;
    this.reloadChart();
  }

  createChart(): void {
    this.tradeService.getPnLData(this.selectedInterval)
      .pipe(map(pnl => pnl.map(p => ({ x: new Date(p.date).getTime(), y: p.balance }))))
      .subscribe({
        next: data => {
          this.chartSeries[0].data = data;
          this.hideSpinner();
        },
        error: error => {
          this.hideSpinner();
          console.error(error);
        }
      })
  }

  reloadChart(): void {
    this.isLoading = true;
    this.chartSeries[0].data = [];
    this.createChart();
  }
}
