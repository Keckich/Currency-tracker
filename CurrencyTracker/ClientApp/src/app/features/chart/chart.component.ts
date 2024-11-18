import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ApexAxisChartSeries, ApexChart, ApexPlotOptions, ApexTitleSubtitle, ApexXAxis, ApexYAxis } from 'ng-apexcharts';
import { BinanceService } from '../../core/services/binance.service';
import { Subscription } from 'rxjs';
import { NgApexchartsModule } from 'ng-apexcharts';
import { CandleData, ChartData, Trade } from '../../shared/shared.model';
import { TradesService } from '../../core/services/trades.service';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Constants } from '../../shared/constants.value';

import { MatButton } from '@angular/material/button';
import { MatLabel, MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatIcon } from '@angular/material/icon';
import { MatOption, MatSelect } from '@angular/material/select';

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
    MatSelect,
    MatOption,
    RouterLink,
  ],
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class ChartComponent implements OnInit, OnDestroy {
  private priceObservable: Subscription = new Subscription;
  private chartObservable: Subscription = new Subscription;
  private currentTrade: Trade | undefined;

  intervals = Constants.CHART_TIME_INTERVALS;
  selectedInterval: string = Constants.CHART_TIME_INTERVALS[0];
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

  private getChartTitle(): string {
    return `${this.currencyPair} Price`;
  }

  ngOnInit(): void {
    this.chartSeries[0].name = this.getChartTitle();
    this.chartOptions.title.text = this.getChartTitle();
    
    this.priceObservable = this.getPrice();
    this.chartObservable = this.createChart();
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
    return this.binanceService.getCryptoCandleData(this.currencyPair, this.selectedInterval).subscribe({
      next: data => {
        if (data.k) {
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
        }
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
    this.chartObservable.unsubscribe();
    this.chartObservable = this.createChart();
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
