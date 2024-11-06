import { Component, OnInit } from '@angular/core';
import { BinanceService } from '../core/services/binance.service';

@Component({
  selector: 'app-chart',
  templateUrl: './chart.component.html',
  styleUrls: ['./chart.component.css']
})
export class ChartComponent implements OnInit {
  cryptoPrice: number | null = null;

  constructor(private binanceService: BinanceService) { }

  ngOnInit(): void {
    this.getPrice();
  }

  getPrice(): void {
    this.binanceService.getCryptoPrice('BTCUSDT').subscribe(
      data => {
        this.cryptoPrice = parseFloat(data.price);
      },
      error => console.log(`Error occured: ${error}`)
    )

    this.binanceService.getAPIInfo().subscribe(
      data => console.log(data)
    )
  }
}
