import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, map, of, shareReplay, switchMap, tap } from 'rxjs';
import { webSocket } from 'rxjs/webSocket';
import { Currency, Order, OrderBook, OrderBookData, TradeSignal } from '../../shared/shared.model';
import { ApiUrlResources, ApiUrls } from '../../shared/constants.value';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class BinanceService {
  private baseUrl!: string;
  private hubConnection!: signalR.HubConnection;
  private currenciesCache: BehaviorSubject<Currency[] | null> = new BehaviorSubject<Currency[] | null>(null);
  private readonly exchangeInfoUrl = 'https://api.binance.com/api/v3/exchangeInfo';
  private readonly pricesUrl = 'https://api.binance.com/api/v3/ticker/price';
  private readonly binanceUrl = ApiUrls.BINANCE;
  private readonly tradeSignalUrl = ApiUrls.TRADE_SIGNAL;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`https://localhost:7213/CryptoHub`, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch(err => console.error(err));
  }

  getCryptoPriceUpdates(crypto: string): Observable<any> {
    return this.http.post(`${this.binanceUrl}/${ApiUrlResources.SUBSCRIBE}`, {
      symbol: crypto,
      type: 'ticker',
    });
  }

  getCryptoCandleData(crypto: string, interval: string): Observable<any> {
    return this.http.post(`${this.binanceUrl}/${ApiUrlResources.SUBSCRIBE}`, {
      symbol: crypto,
      type: 'kline',
      interval: interval,
    });
  }

  unsubscribeCryptoCandleData(crypto: string, interval: string): Observable<any> {
    return this.http.post(`${this.binanceUrl}/${ApiUrlResources.UNSUBSCRIBE}`, {
      symbol: crypto,
      type: 'kline',
      interval: interval,
    });
  }

  getOrderBookData(crypto: string): Observable<any> {
    return this.http.post(`${this.binanceUrl}/${ApiUrlResources.SUBSCRIBE}`, {
      symbol: crypto,
      type: 'depth',
    });
  }

  unsubscribeOrderBookData(crypto: string): Observable<any> | void {
    if (crypto) {
      return this.http.post(`${this.binanceUrl}/${ApiUrlResources.UNSUBSCRIBE}`, {
        symbol: crypto,
        type: 'depth',
      });
    }
  }

  onPriceUpdates(callback: (data: any) => void) {
    this.hubConnection.on('Receive_ticker', (rawData: any) => {
      const data = JSON.parse(rawData);
      callback(data);
    });
  }

  onCandleData(callback: (data: any) => void) {
    this.hubConnection.on('Receive_kline', (rawData: any) => {
      const data = JSON.parse(rawData);
      callback(data);
    });
  }

  onOrderBook(callback: (data: any) => void) {
    this.hubConnection.on('Receive_depth', (rawData: any) => {
      const data = JSON.parse(rawData);
      const transformedData: OrderBook = {
        asks: data.a.map(([price, amount]: [number, number]) => ({ price, amount })).slice(0, 10).sort((a: Order, b: Order) => b.price - a.price),
        bids: data.b.map(([price, amount]: [number, number]) => ({ price, amount })).slice(0, 10).sort((a: Order, b: Order) => b.price - a.price),
      };

      callback(transformedData);
    });
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

  getTradeSignals(crypto: string, interval: string): Observable<any> {
    return this.http.post(`${this.tradeSignalUrl}/${ApiUrlResources.SUBSCRIBE}`, {
      symbol: crypto,
      type: 'kline',
      interval: interval,
    });
  }

  unsubscribeTradeSignals(crypto: string, interval: string): Observable<any> {
    return this.http.post(`${this.tradeSignalUrl}/${ApiUrlResources.UNSUBSCRIBE}`, {
      symbol: crypto,
      type: 'kline',
      interval: interval,
    });
  }

  onTradeSignalData(callback: (data: TradeSignal) => void) {
    this.hubConnection.on('ReceiveTradeSignal', (rawData: any) => {
      const data = JSON.parse(rawData);
      callback(data);
    });
  }
}
