import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { BinanceService } from '../../core/services/binance.service';
import { ChartInterval, TradeSignalType } from '../../shared/shared.enum';
import { TradeSignal } from '../../shared/shared.model';
import { formatString } from '../../shared/utilities';
import { Constants, TradeSignals } from '../../shared/constants.value';

@Component({
  selector: 'app-trade-signal',
  standalone: true,
  imports: [],
  templateUrl: './trade-signal.component.html',
  styleUrl: './trade-signal.component.css'
})
export class TradeSignalComponent implements OnInit, OnDestroy {
  private signalSubscription!: Subscription;

  tradeSignal: string = Constants.WAITING_SIGNAL;
  @Input() currencyPair!: string;
  @Input() selectedInterval!: ChartInterval;

  constructor(private binanceService: BinanceService) { }

  ngOnInit(): void {
    this.binanceService.unsubscribeTradeSignals(this.currencyPair, this.selectedInterval).subscribe();
    this.signalSubscription = this.binanceService.getTradeSignals(this.currencyPair, this.selectedInterval).subscribe({
      next: data => console.log(data),
      error: (error) => {
        console.error('WebSocket error:', error);
        this.tradeSignal = Constants.CONNECTION_LOST_SIGNAL;
      }
    });

    this.binanceService.onTradeSignalData((data: TradeSignal) => {
      this.tradeSignal = formatString(Constants.TRADE_SIGNAL, TradeSignals[data.Type], this.formatConfidence(data));
    })
  }

  private formatConfidence(data: TradeSignal): number {
    switch (data.Type) {
      case TradeSignalType.Sell:
      case TradeSignalType.Neutral:
        return 100 - data.Confidence;
      default:
        return data.Confidence;
    }
  }

  ngOnDestroy(): void {
    this.binanceService.unsubscribeTradeSignals(this.currencyPair, this.selectedInterval).subscribe();
    if (this.signalSubscription) {
      this.signalSubscription.unsubscribe();
    }
  }
}
