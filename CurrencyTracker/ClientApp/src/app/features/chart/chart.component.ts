import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild, inject } from '@angular/core';
import { ApexAxisChartSeries, ApexChart, ApexPlotOptions, ApexTitleSubtitle, ApexXAxis, ApexYAxis } from 'ng-apexcharts';
import { BinanceService } from '../../core/services/binance.service';
import { Observable, Subscription, filter } from 'rxjs';
import { NgApexchartsModule } from 'ng-apexcharts';
import { CandleData, ChartData, RouteParams, Trade } from '../../shared/shared.model';
import { ChartIntervals } from '../../shared/constants.value';
import { TradesService } from '../../core/services/trades.service';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ChartOptions, Constants, Routes } from '../../shared/constants.value';
import { LimitOrderComponent } from '../limit-order/limit-order.component';

import { MatButton } from '@angular/material/button';
import { MatLabel, MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatIcon } from '@angular/material/icon';
import { MatOption, MatSelect } from '@angular/material/select';
import { MatCheckbox } from '@angular/material/checkbox';
import { CommonModule } from '@angular/common';
import { TransactionComponent } from '../transaction/transaction.component';
import { IntervalListComponent } from '../interval-list/interval-list.component';
import { ChartInterval } from '../../shared/shared.enum';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { NgxSpinnerService } from 'ngx-spinner';
import { RouteService } from '../../core/services/route.service';

@Component({
  selector: 'app-chart',
  standalone: true,
  imports: [
    NgApexchartsModule,
    FormsModule,
    CommonModule,
    MatInput,
    MatLabel,
    MatFormField,
    MatSelect,
    MatOption,
    RouterLink,
    TransactionComponent,
    IntervalListComponent,
    LoadingSpinnerComponent,
  ],
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class ChartComponent implements OnInit, OnDestroy {
  private binanceService = inject(BinanceService);
  private tradesService = inject(TradesService);
  private spinner = inject(NgxSpinnerService);
  private routeService = inject(RouteService);
  private chartSubscription: Subscription = new Subscription;

  readonly Routes = Routes;
  selectedInterval: ChartInterval = ChartInterval.S1;
  isLoading: boolean = true;
  chartIntervals: Partial<Record<ChartInterval, string>> = ChartIntervals;
  @Input() currencyPair!: string;
  @Output() intervalChange = new EventEmitter<ChartInterval>();

  chartSeries: ApexAxisChartSeries = [
    {
      name: '',
      data: []
    }
  ]

  chartOptions = ChartOptions;

  private getChartTitle(): string {
    return $localize`:@@chartTitle:${this.currencyPair} Price`;
  }

  ngOnInit(): void {
    this.routeService.params$
      .subscribe(params => {
        this.loadData(params)
      });
  }

  loadData(params: RouteParams | null): void {
    this.isLoading = true;
    if (params?.id) {
      this.currencyPair = params?.id!;
    }

    this.spinner.show();
    this.chartSeries[0].name = this.getChartTitle();
    this.chartOptions.title.text = this.getChartTitle();

    this.reloadChart();
  }

  createChart(): Subscription {
    return this.binanceService.getCryptoCandleData(this.currencyPair, this.selectedInterval)
      .subscribe({
        next: data => {
          this.binanceService.onCandleData(data => {
            if (data.k) {
              const candle = this.createCandle(data);
              this.upateCandleChart(candle);

              if (this.isLoading) {
                this.isLoading = false;
                this.spinner.hide();
              }
            }
          })
        },
        error: error => {
          console.log(`Error occured: ${JSON.stringify(error)}`);
          this.spinner.hide();
        }
    })
  }

  private createCandle(data: any): ChartData {
    const kline = data.k;
    const candle = {
      x: kline.t,
      y: {
        open: parseFloat(kline.o),
        high: parseFloat(kline.h),
        low: parseFloat(kline.l),
        close: parseFloat(kline.c),
      } as CandleData
    };

    const apexCandle: ChartData = {
      x: candle.x,
      y: [candle.y.open, candle.y.high, candle.y.low, candle.y.close],
    }

    return apexCandle;
  }

  private upateCandleChart(candle: ChartData): void {
    (this.chartSeries[0].data as ChartData[]).push(candle);

    if (this.chartSeries[0]?.data.length > Constants.CANDLE_CHART_SIZE) {
      this.chartSeries[0]?.data.shift();
    }

    this.chartSeries = [... this.chartSeries];
  }

  onIntervalChange(interval: ChartInterval) {
    this.binanceService.unsubscribeCryptoCandleData(this.currencyPair, this.selectedInterval).subscribe();
    this.selectedInterval = interval;
    this.intervalChange.emit(interval);
    this.reloadChart();
  }

  reloadChart(): void {
    this.chartSeries = [
      {
        name: this.getChartTitle(),
        data: []
      }
    ];
    this.chartSubscription.unsubscribe();
    this.chartSubscription = this.createChart();
  }

  ngOnDestroy(): void {
    if (this.chartSubscription) {
      this.chartSubscription.unsubscribe();
    }
  }
}
