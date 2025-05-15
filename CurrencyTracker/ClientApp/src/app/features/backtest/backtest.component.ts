import { Component } from '@angular/core';
import { BacktestService } from '../../core/services/backtest.service';

@Component({
  selector: 'app-backtest',
  standalone: true,
  imports: [],
  templateUrl: './backtest.component.html',
  styleUrl: './backtest.component.css'
})
export class BacktestComponent {
  isLoading = false;
  result: any = null;

  constructor(private backtestService: BacktestService) {}

  runBacktest() {
    this.isLoading = true;
    this.result = null;

    this.backtestService.runBacktest({
      symbol: 'BTCUSDT',
      interval: '4h',
      start: '2024-01-01',
      end: '2024-05-01'
    }).subscribe({
      next: data => {
        this.result = data;
        this.isLoading = false;
      },
      error: err => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }
}
