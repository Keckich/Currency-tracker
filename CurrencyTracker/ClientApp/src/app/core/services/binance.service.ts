import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BinanceService {
  private apiUrl = 'https://api.binance.com/api/v3/ticker/price';

  constructor(private http: HttpClient) { }

  getCryptoPrice(crypto: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}?symbol=${crypto}`);
  }

  getAPIInfo(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}`);
  }
}
