import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { ApexAxisChartSeries, ApexChart, ApexPlotOptions, ApexTitleSubtitle, ApexXAxis, ApexYAxis } from 'ng-apexcharts';
import { BinanceService } from '../../core/services/binance.service';
import { Observable, Subscription, filter } from 'rxjs';
import { NgApexchartsModule } from 'ng-apexcharts';
import { CandleData, ChartData, Trade } from '../../shared/shared.model';
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
  ],
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class ChartComponent implements OnInit, OnDestroy {
  private chartSubscription: Subscription = new Subscription;

  Routes = Routes;
  intervals = Object.entries(ChartIntervals).map(([key, { value, display }]) => ({ key, value, display }));
  selectedInterval: string = ChartIntervals.S1.value;
  @Input() currencyPair!: string;

  chartSeries: ApexAxisChartSeries = [
    {
      name: '',
      data: []
    }
  ]

  chartOptions = ChartOptions;

  constructor(private binanceService: BinanceService, private tradesService: TradesService) { }

  private getChartTitle(): string {
    return $localize`:@@chartTitle:${this.currencyPair} Price`;
  }

  ngOnInit(): void {
    this.chartSeries[0].name = this.getChartTitle();
    this.chartOptions.title.text = this.getChartTitle();
    
    this.chartSubscription = this.createChart();
  }

  createChart(): Subscription {
    return this.binanceService.getCryptoCandleData(this.currencyPair, this.selectedInterval)
      .pipe(filter(data => data.k))
      .subscribe({
        next: data => {
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

          this.upateCandleChart(apexCandle);
        },
        error: error => console.log(`Error occured: ${error}`)
    })
  }

  private upateCandleChart(candle: ChartData): void {
    (this.chartSeries[0].data as ChartData[]).push(candle);

    if (this.chartSeries[0]?.data.length > Constants.CANDLE_CHART_SIZE) {
      this.chartSeries[0]?.data.shift();
    }

    this.chartSeries = [... this.chartSeries];
  }

  onIntervalChange(event: any): void {
    const newIntreval = event.value;
    this.selectedInterval = newIntreval;
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
