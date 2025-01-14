import { Injectable, inject } from '@angular/core';
import { HttpService } from './http.service';
import { Observable } from 'rxjs';
import { OrderBook } from '../../shared/shared.model';
import { ApiUrls } from '../../shared/constants.value';

@Injectable({
  providedIn: 'root'
})
export class OrderBookService {
  private httpService = inject(HttpService);

  getOrderBook(symbol: string, limit: number = 50): Observable<OrderBook> {
    return this.httpService.get<OrderBook>(ApiUrls.ORDER_BOOK + `/${symbol}`, { limit: limit });
  }
}
