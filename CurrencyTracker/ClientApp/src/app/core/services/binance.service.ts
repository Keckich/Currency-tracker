import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { webSocket } from 'rxjs/webSocket';

@Injectable({
  providedIn: 'root'
})
export class BinanceService {
  private binanceSocketUrl(cryptoPair: string): string {
    return `wss://stream.binance.com:9443/ws/${cryptoPair.toLowerCase()}@ticker`;
  }

  constructor(private http: HttpClient) { }

  getCryptoPriceUpdates(crypto: string): Observable<any> {
    const socket = webSocket(this.binanceSocketUrl(crypto));
    return socket;
  }
}
