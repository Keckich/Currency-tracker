import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ApexAxisChartSeries, ApexChart, ApexPlotOptions, ApexTitleSubtitle, ApexXAxis, ApexYAxis } from 'ng-apexcharts';
import { BinanceService } from '../../core/services/binance.service';
import { Subscription } from 'rxjs';
import { NgApexchartsModule } from 'ng-apexcharts';
import { CandleData, Trade } from '../../shared/shared.model';
import { TradesService } from '../../core/services/trades.service';
import { FormsModule } from '@angular/forms';

import { MatButton } from '@angular/material/button';
import { MatLabel, MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { Constants } from '../../shared/constants.value';

@Component({
  selector: 'app-chart',
  standalone: true,
  imports: [
    NgApexchartsModule,
    FormsModule,
    MatButton,
    MatInput,
    MatLabel,
    MatIcon,
    MatFormField,
    RouterLink,
  ],
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class ChartComponent implements OnInit, OnDestroy {
  private priceObservable: Subscription = new Subscription;
  private chartObservable: Subscription = new Subscription;
  private currentTrade: Trade | undefined;

  amount: number = 0;
  @Input() currencyPair!: string;

  chartSeries: ApexAxisChartSeries = [
    {
      name: '',
      data: []
    }
  ]

  chartOptions: {
    chart: ApexChart,
    xaxis: ApexXAxis,
    yaxis: ApexYAxis,
    title: ApexTitleSubtitle,
    plotOptions: ApexPlotOptions,
  } = {
    chart: {
      type: 'candlestick',
      height: 350,
      animations: {
        enabled: false,
      },
      offsetX: 0,
    },
    xaxis: {
      type: 'datetime',
      tickPlacement: 'on',
    },
    yaxis: {
      tooltip: {
        enabled: true,
      },
    },
    title: {
      text: '',
    },
    plotOptions: {
      candlestick: {
        colors: {
          upward: Constants.CANDLE_COLORS.green,
          downward: Constants.CANDLE_COLORS.red,
        },
      }
    }
  }

  constructor(private binanceService: BinanceService, private tradesService: TradesService) { }

  ngOnInit(): void {
    this.chartSeries[0].name = `${this.currencyPair} Price`;
    this.chartOptions.title.text = `${this.currencyPair} Price`;
    
    this.priceObservable = this.getPrice();
    this.createChart();
  }

  getPrice(): Subscription {
    return this.binanceService.getCryptoPriceUpdates(this.currencyPair).subscribe({
      next: data => {
        const currentDate = new Date();
        const price = parseFloat(data.c);

        this.setCurrentTradeValue(currentDate, price);
      },
      error: error => console.log(`Error occured: ${error}`)
    })
  }

  private setCurrentTradeValue(currentDate: Date, price: number): void {
    this.currentTrade = {
      price: price,
      date: currentDate,
      currency: this.currencyPair,
      amount: this.amount,
    };
  }

  createChart(): Subscription {
    return this.binanceService.getCryptoCandleData(this.currencyPair, '1s').subscribe({
      next: data => {
        if (data.k) {
          const kline = data.k;
          const candle: CandleData[] = [
            kline.t,
            parseFloat(kline.o),
            parseFloat(kline.h),
            parseFloat(kline.l),
            parseFloat(kline.c),
          ];

          this.upateCandleChart(candle);
        }
      },
      error: error => console.log(`Error occured: ${error}`)
    })
  }

  private upateCandleChart(candle: any): void {
    this.chartSeries[0]?.data.push(candle);

    if (this.chartSeries[0]?.data.length > Constants.CANDLE_CHART_SIZE) {
      this.chartSeries[0]?.data.shift();
    }

    this.chartSeries = [... this.chartSeries];
  }

  buy(): void {
    this.tradesService.addTrade(this.currentTrade);
  }

  ngOnDestroy(): void {
    if (this.priceObservable) {
      this.priceObservable.unsubscribe();
    }

    if (this.chartObservable) {
      this.chartObservable.unsubscribe();
    }
  }
}
