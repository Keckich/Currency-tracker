import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ApexAxisChartSeries, ApexChart, ApexTitleSubtitle, ApexXAxis } from 'ng-apexcharts';
import { BinanceService } from '../../core/services/binance.service';
import { Subscription } from 'rxjs';
import { NgApexchartsModule } from 'ng-apexcharts';
import { ChartData, Trade } from '../../shared/shared.model';
import { TradesService } from '../../core/services/trades.service';
import { FormsModule } from '@angular/forms';

import { MatButton } from '@angular/material/button';
import { MatLabel, MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';

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
  private currentTrade: Trade | undefined;

  amount: number = 0;
  @Input() currencyPair!: string;
  @Output() buyEvent = new EventEmitter<Trade[]>();

  chartSeries: ApexAxisChartSeries = [
    {
      name: '',
      data: [] as ChartData[]
    }
  ]

  chartOptions: {
    chart: ApexChart,
    xaxis: ApexXAxis,
    title: ApexTitleSubtitle,
  } = {
    chart: {
      type: 'line',
      height: 350
    },
    xaxis: {
      type: 'datetime',
    },
    title: {
      text: '',
    }
  }

  constructor(private binanceService: BinanceService, private tradesService: TradesService) { }

  ngOnInit(): void {
    this.chartSeries[0].name = `${this.currencyPair} Price`;
    this.chartOptions.title.text = `${this.currencyPair} Price`;
    
    this.priceObservable = this.getPrice();
  }

  getPrice(): Subscription {
    return this.binanceService.getCryptoPriceUpdates(this.currencyPair).subscribe({
      next: data => {
        const price = parseFloat(data.c);
        const currentTime = new Date().getTime();
        this.currentTrade = {
          price: price,
          date: new Date(),
          currency: this.currencyPair,
          amount: this.amount,
        };

        (this.chartSeries[0].data as ChartData[]).push({ x: currentTime, y: price });

        if (this.chartSeries[0].data.length > 20) {
          this.chartSeries[0].data.shift();
        }

        this.chartSeries = [...this.chartSeries];
      },
      error: error => console.log(`Error occured: ${error}`)
    })
  }

  buy(): void {
    this.tradesService.addTrade(this.currentTrade);
  }

  ngOnDestroy(): void {
    if (this.priceObservable) {
      this.priceObservable.unsubscribe();
    }
  }
}
