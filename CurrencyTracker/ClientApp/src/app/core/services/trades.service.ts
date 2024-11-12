import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Trade } from '../../shared/shared.model';

@Injectable({
  providedIn: 'root'
})
export class TradesService {
  private tradesSubject = new BehaviorSubject<Trade[]>([]);
  trades$: Observable<Trade[]> = this.tradesSubject.asObservable();

  addTrade(trade: Trade | undefined): void {
    if (trade) {
      const currentTrades = this.tradesSubject.value;
      this.tradesSubject.next([...currentTrades, trade]);
    }
  }
}
