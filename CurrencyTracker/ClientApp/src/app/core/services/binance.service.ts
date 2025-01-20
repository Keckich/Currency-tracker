import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map, of, shareReplay, switchMap, tap } from 'rxjs';
import { webSocket } from 'rxjs/webSocket';
import { Currency, Order, OrderBook, OrderBookData } from '../../shared/shared.model';

@Injectable({
  providedIn: 'root'
})
export class BinanceService {
  private currenciesCache: BehaviorSubject<Currency[] | null> = new BehaviorSubject<Currency[] | null>(null);
  private readonly exchangeInfoUrl = 'https://api.binance.com/api/v3/exchangeInfo';
  private readonly pricesUrl = 'https://api.binance.com/api/v3/ticker/price';
  private binanceSocketUrl(cryptoPair: string): string {
    return `wss://stream.binance.com:9443/ws/${cryptoPair?.toLowerCase()}@ticker`;
  }

  private binanceCandleSocketUrl(cryptoPair: string, interval: string): string {
    return `wss://stream.binance.com:9443/ws/${cryptoPair?.toLowerCase()}@kline_${interval}`;
  }

  private binanceOrderBookSocketUrl(cryptoPair: string): string {
    return `wss://stream.binance.com:9443/ws/${cryptoPair?.toLowerCase()}@depth`;
  }

  constructor(private http: HttpClient) { }

  getCryptoPriceUpdates(crypto: string): Observable<any> {
    const socket = webSocket(this.binanceSocketUrl(crypto));
    return socket;
  }

  getCryptoCandleData(crypto: string, interval: string): Observable<any> {
    const socket = webSocket(this.binanceCandleSocketUrl(crypto, interval));
    return socket;
  }

  getOrderBookData(crypto: string): Observable<OrderBook> {
    const socket = webSocket<OrderBookData>(this.binanceOrderBookSocketUrl(crypto)).pipe(
      map(data => ({
        asks: data.a.map(([price, amount]) => ({ price, amount })).slice(0, 10).sort((a, b) => b.price - a.price),
        bids: data.b.map(([price, amount]) => ({ price, amount })).slice(0, 10).sort((a, b) => b.price - a.price),
      }))
    );

    return socket;
  }

  getCryptocurrencies(): Observable<Currency[]> {
    return this.currenciesCache.pipe(
      switchMap(cached => {
        if (cached) {
          return of(cached);
        } else {
          return this.http.get<Currency[]>(this.exchangeInfoUrl).pipe(
            map((response: any) =>
              [...response.symbols as Currency[]]
            ),
            tap((currencies: Currency[]) => this.currenciesCache.next(currencies))
          );
        }
      })
    );
  }

  clearCache(): void {
    this.currenciesCache.next(null);
  }

  getPrices(currencies: string[]): Observable<Record<string, number>> {
    return this.http.get<{ symbol: string; price: string }[]>(this.pricesUrl).pipe(
      map(prices =>
        prices
          .filter(price => currencies.includes(price.symbol))
          .reduce((acc, price) => {
            acc[price.symbol] = parseFloat(price.price);
            return acc;
          }, {} as Record<string, number>)
      )
    );
  }
}
