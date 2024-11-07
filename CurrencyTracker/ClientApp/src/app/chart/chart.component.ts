import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ApexAxisChartSeries, ApexChart, ApexTitleSubtitle, ApexXAxis } from 'ng-apexcharts';
import { BinanceService } from '../core/services/binance.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class ChartComponent implements OnInit, OnDestroy {
  @Input() currencyPair!: string;
  private priceObservable: Subscription = new Subscription;

  public chartSeries: ApexAxisChartSeries = [
    {
      name: '',
      data: [] as { x: number, y: number }[]
    }
  ]

  public chartOptions: {
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

  constructor(private binanceService: BinanceService) { }

  ngOnInit(): void {
    this.chartSeries[0].name = `${this.currencyPair} Price`;
    this.chartOptions.title.text = `${this.currencyPair} Price`;

    this.priceObservable = this.getPrice();
  }

  getPrice(): Subscription {
    return this.binanceService.getCryptoPriceUpdates(this.currencyPair).subscribe(
      data => {
        const price = parseFloat(data.c);
        const currentTime = new Date().getTime();

        (this.chartSeries[0].data as { x: number, y: number }[]).push({ x: currentTime, y: price });

        if (this.chartSeries[0].data.length > 20) {
          this.chartSeries[0].data.shift();
        }

        this.chartSeries = [...this.chartSeries];
      },
      error => console.log(`Error occured: ${error}`)
    )
  }

  ngOnDestroy(): void {
    if (this.priceObservable) {
      this.priceObservable.unsubscribe();
    }
  }
}
