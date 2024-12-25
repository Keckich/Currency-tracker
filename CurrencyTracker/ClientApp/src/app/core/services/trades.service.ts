import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';
import { AnalysisResult, AnalyzedTradeInfo, Trade } from '../../shared/shared.model';
import { AnalysisRecommenations } from '../../shared/constants.value';

@Injectable({
  providedIn: 'root'
})
export class TradesService {
  private tradesSubject = new BehaviorSubject<Trade[]>([]);
  private pricesSubject = new BehaviorSubject<Record<string, number>>({});

  trades$: Observable<Trade[]> = this.tradesSubject.asObservable();
  prices$: Observable<Record<string, number>> = this.pricesSubject.asObservable();

  addTrade(trade: Trade | undefined): void {
    if (trade) {
      const currentTrades = this.tradesSubject.value;
      this.tradesSubject.next([...currentTrades, trade]);
    }
  }

  getTrades(): Trade[] {
    return this.tradesSubject.value;
  }

  updatePrices(prices: Record<string, number>): void {
    const currentPrices = this.pricesSubject.value;
    this.pricesSubject.next({ ...currentPrices, ...prices });
  }

  analyzedTrades$: Observable<AnalysisResult[]> = combineLatest([this.trades$, this.prices$]).pipe(
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
      }, {} as Record<string, AnalyzedTradeInfo>);

      return this.analyzeTrades(grouped, prices);
    })
  )

  analyzeTrades(grouped: Record<string, AnalyzedTradeInfo>, prices: Record<string, number>): AnalysisResult[] {
    return Object.keys(grouped).map(currency => {
      const { totalAmount, totalSpent } = grouped[currency];
      const avgPrice = totalSpent / totalAmount;
      const currentPrice = prices[currency] || 0;
      const roi = ((currentPrice - avgPrice) / avgPrice) * 100;
      const recommendation = this.getRecommendation(roi);

      return {
        tradeInfo: {
          totalAmount: totalAmount,
          totalSpent: totalSpent,
        } as AnalyzedTradeInfo,
        currency: currency,
        avgPrice: avgPrice,
        roi: roi,
        recommendation: recommendation,
      } as AnalysisResult;
    })
  }

  getRecommendation(roi: number): string {
    let recommendation = '';
    if (roi > 50) {
      recommendation = AnalysisRecommenations.SELL_PROFIT;
    } else if (roi > 10) {
      recommendation = AnalysisRecommenations.KEEP_GROWTH;
    } else if (roi < 0) {
      recommendation = AnalysisRecommenations.SELL_LOSS;
    } else {
      recommendation = AnalysisRecommenations.KEEP_RECOVER;
    }

    return recommendation;
  }
}
