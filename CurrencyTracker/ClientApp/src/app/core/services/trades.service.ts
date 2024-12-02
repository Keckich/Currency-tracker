import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Trade } from '../../shared/shared.model';

@Injectable({
  providedIn: 'root'
})
export class TradesService {
  private tradesSubject = new BehaviorSubject<Partial<Trade>[]>([]);
  trades$: Observable<Partial<Trade>[]> = this.tradesSubject.asObservable();

  addTrade(trade: Partial<Trade> | undefined): void {
    if (trade) {
      const currentTrades = this.tradesSubject.value;
      this.tradesSubject.next([...currentTrades, trade]);
    }
  }
}
