import { Component } from '@angular/core';
import { MatDivider } from '@angular/material/divider';
import { ChartComponent } from '../features/chart/chart.component';
import { TradesComponent } from '../features/trades/trades.component';
import { Trade } from '../shared/models/trade';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    ChartComponent,
    TradesComponent,
    MatDivider,
  ],
  templateUrl: './home.component.html',
})
export class HomeComponent {
  trades!: Trade[];
  public currencies = ['BTCUSDT', 'ETHUSDT', 'BNBUSDT'];

  handleBuy(trades: Trade[]): void {
    this.trades = trades;
  }
}
