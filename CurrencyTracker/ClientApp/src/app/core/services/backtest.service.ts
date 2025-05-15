import { Injectable } from '@angular/core';
import { HttpService } from './http.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BacktestService {
  constructor(private httpService: HttpService) {}

  runBacktest(params: {
    symbol: string;
    interval: string;
    start: string;
    end: string;
  }): Observable<any> {
    return this.httpService.post('/api/backtest', params);
  }
}
