import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { webSocket } from 'rxjs/webSocket';
import { Currency } from '../../shared/shared.model';

@Injectable({
  providedIn: 'root'
})
export class BinanceService {
  private readonly exchangeInfoUrl = 'https://api.binance.com/api/v3/exchangeInfo';
  private readonly pricesUrl = 'https://api.binance.com/api/v3/ticker/price';
  private binanceSocketUrl(cryptoPair: string): string {
    return `wss://stream.binance.com:9443/ws/${cryptoPair?.toLowerCase()}@ticker`;
  }
  private binanceCandleSocketUrl(cryptoPair: string, interval: string): string {
    return `wss://stream.binance.com:9443/ws/${cryptoPair?.toLowerCase()}@kline_${interval}`;
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

  getCryptocurrencies(): Observable<Currency[]> {
    return this.http.get(this.exchangeInfoUrl).pipe(
      map((response: any) =>
        [...response.symbols as Currency[]]
      )
    );
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
