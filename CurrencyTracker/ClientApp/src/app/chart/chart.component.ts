import { Component, OnInit } from '@angular/core';
import { ApexAxisChartSeries, ApexChart, ApexTitleSubtitle, ApexXAxis } from 'ng-apexcharts';
import { BinanceService } from '../core/services/binance.service';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class ChartComponent implements OnInit {
  cryptoPrice: number | null = null;

  public chartSeries: ApexAxisChartSeries = [
    {
      name: 'BTC price',
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
      text: 'BTC/USDT Price',
    }
  }

  constructor(private binanceService: BinanceService) { }

  ngOnInit(): void {
    this.getPrice();
    setInterval(() => this.getPrice(), 1000);
  }

  getPrice(): void {
    this.binanceService.getCryptoPrice('BTCUSDT').subscribe(
      data => {
        const price = parseFloat(data.price);
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
}
