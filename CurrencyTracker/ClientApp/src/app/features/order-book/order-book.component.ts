import { ChangeDetectionStrategy, Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, inject } from '@angular/core';
import { BinanceService } from '../../core/services/binance.service';
import { MatTableModule } from '@angular/material/table';
import { MatDividerModule } from '@angular/material/divider';
import { Constants } from '../../shared/constants.value';
import { CommonModule } from '@angular/common';
import { Order } from '../../shared/shared.model';
import { Subscription, filter } from 'rxjs';
import { RouteService } from '../../core/services/route.service';

@Component({
  selector: 'app-order-book',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatDividerModule,
  ],
  templateUrl: './order-book.component.html',
  styleUrl: './order-book.component.css',
})
export class OrderBookComponent implements OnInit, OnDestroy {
  private binanceService = inject(BinanceService);
  private routeService = inject(RouteService);
  private socketSubscription?: Subscription;

  bids: Order[] = [];
  asks: Order[] = [];
  displayedColumns: string[] = Constants.ORDER_BOOK_COLUMNS;
  @Input() currencyPair!: string;

  ngOnInit(): void {
    this.routeService.params$
      .pipe(filter(params => params?.id != null))
      .subscribe(params => {
        this.currencyPair = params!.id!
        this.loadData();
      });
  }

  loadData(): void {
    this.binanceService.unsubscribeOrderBookData(this.currencyPair)?.subscribe();
    this.socketSubscription?.unsubscribe();

    this.socketSubscription = this.binanceService.getOrderBookData(this.currencyPair).subscribe();
    this.binanceService.onOrderBook(data => {
      this.bids = data.bids;
      this.asks = data.asks;
    });
  }

  ngOnDestroy(): void {
    this.binanceService.unsubscribeOrderBookData(this.currencyPair)?.subscribe();
    this.socketSubscription?.unsubscribe();
  }
}
