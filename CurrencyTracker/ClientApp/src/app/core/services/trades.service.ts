import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';
import { Trade } from '../../shared/shared.model';

@Injectable({
  providedIn: 'root'
})
export class TradesService {
  private tradesSubject = new BehaviorSubject<Partial<Trade>[]>([]);
  private pricesSubject = new BehaviorSubject<Record<string, number>>({});

  trades$: Observable<Partial<Trade>[]> = this.tradesSubject.asObservable();
  prices$: Observable<Record<string, number>> = this.pricesSubject.asObservable();

  addTrade(trade: Partial<Trade> | undefined): void {
    if (trade) {
      const currentTrades = this.tradesSubject.value;
      this.tradesSubject.next([...currentTrades, trade]);
    }
  }

  getTrades(): Partial<Trade>[] {
    return this.tradesSubject.value;
  }

  updatePrices(prices: Record<string, number>): void {
    const currentPrices = this.pricesSubject.value;
    this.pricesSubject.next({ ...currentPrices, ...prices });
  }

  analyzedTrades$ = combineLatest([this.trades$, this.prices$]).pipe(
    map(([trades, prices]) => {
      const grouped = trades.reduce((groups, trade) => {
        const key = trade.currency;
        if (key) {
          if (!groups[key]) {
            groups[key] = { totalAmount: 0, totalSpent: 0 };
          }

          groups[key].totalAmount += trade.amount!;
          groups[key].totalSpent += trade.value!;
        }

        return groups;

        return groups;
      }, {} as Record<string, { totalAmount: number, totalSpent: number }>);

      // TODO: add separate types for this type of trades and try to update roi in real time
      return Object.keys(grouped).map(currency => {
        const { totalAmount, totalSpent } = grouped[currency];
        const avgPrice = totalSpent / totalAmount;
        const currentPrice = prices[currency] || 0;
        const roi = ((currentPrice - avgPrice) / avgPrice) * 100;

        return { currency, avgPrice, totalAmount, roi };
      })
    })
  )
}
