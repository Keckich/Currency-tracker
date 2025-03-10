import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { BinanceService } from '../../core/services/binance.service';

@Component({
  selector: 'app-trade-signal',
  standalone: true,
  imports: [],
  templateUrl: './trade-signal.component.html',
  styleUrl: './trade-signal.component.css'
})
export class TradeSignalComponent implements OnInit, OnDestroy {
  tradeSignal: string = 'Waiting for signal...';
  private signalSubscription!: Subscription;

  constructor(private binanceService: BinanceService) { }

  ngOnInit(): void {
    this.signalSubscription = this.binanceService.getTradeSignals().subscribe(
      (data) => {
        this.tradeSignal = `Trade Signal: ${data.signal}`;
      },
      (error) => {
        console.error('WebSocket error:', error);
        this.tradeSignal = 'Connection lost!';
      }
    );
  }

  ngOnDestroy(): void {
    if (this.signalSubscription) {
      this.signalSubscription.unsubscribe();
    }
  }
}
