import { Inject, Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, combineLatest, of } from 'rxjs';
import { map, switchMap, tap } from 'rxjs/operators';
import { AnalysisResult, AnalyzedTradeInfo, PnLData, Trade, TradesPaginationData } from '../../shared/shared.model';
import { AnalysisRecommenations, ApiUrls } from '../../shared/constants.value';
import { HttpClient } from '@angular/common/http';
import { HttpService } from './http.service';
import { BinanceService } from './binance.service';

@Injectable({
  providedIn: 'root'
})
export class TradesService {
  private httpService = inject(HttpService);
  private binanceService = inject(BinanceService);
  private tradesSubject = new BehaviorSubject<TradesPaginationData>({ data: [], totalItems: 0 });
  private pricesSubject = new BehaviorSubject<Record<string, number>>({});

  trades$: Observable<TradesPaginationData> = this.tradesSubject.asObservable();
  prices$: Observable<Record<string, number>> = this.pricesSubject.asObservable();
  lastPage: number = 0;
  pageSize: number = 10;

  addTrade(trade: Trade | undefined): void {
    if (trade) {
      this.httpService.post<Trade>(ApiUrls.TRADES, trade)
        .pipe(
          tap(() => {
            this.getPaginatedTrades(this.lastPage, this.pageSize).subscribe();
          })
        )
        .subscribe();
    }
  }

  getPaginatedTrades(page: number = 0, pageSize: number = 10): Observable<TradesPaginationData> {
    const params = { page: page, pageSize: pageSize };
    this.updatePaginationData(page, pageSize);

    return this.httpService.get<TradesPaginationData>(ApiUrls.PAGINATED_TRADES, params).pipe(
      tap(trades => this.tradesSubject.next(trades))
    );
  }

  getTrades(): Observable<Trade[]> {
    return this.httpService.get<Trade[]>(ApiUrls.TRADES);
  }

  getPnLData(interval: number): Observable<PnLData[]> {
    return this.httpService.get<PnLData[]>(ApiUrls.TRADES + '/pnl', { interval: interval });
  }

  updatePaginationData(page: number, pageSize: number) {
    this.lastPage = page;
    this.pageSize = pageSize;
  }

  getTradesValue(): Trade[] {
    return this.tradesSubject.value.data;
  }

  updatePrices(prices: Record<string, number>): void {
    const currentPrices = this.pricesSubject.value;
    this.pricesSubject.next({ ...currentPrices, ...prices });
  }

  analyzedTrades$: Observable<AnalysisResult[]> = this.getTrades().pipe(
    map(trades => {
      const currencies = Array.from(new Set(trades.map(trade => trade.currency).filter(Boolean)));
      return { trades, currencies };
    }),
    switchMap(({ trades, currencies }) =>
      combineLatest([
        of(trades),
        this.binanceService.getPrices(currencies)
      ])
    ),
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
