import { Component, Input, OnInit, inject } from '@angular/core';
import { BinanceService } from '../../core/services/binance.service';
import { MatTableModule } from '@angular/material/table';
import { MatDividerModule } from '@angular/material/divider';
import { Constants } from '../../shared/constants.value';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-order-book',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatDividerModule,
  ],
  templateUrl: './order-book.component.html',
  styleUrl: './order-book.component.css'
})
export class OrderBookComponent implements OnInit {
  private binanceService = inject(BinanceService);

  bids: any[] = [];
  asks: any[] = [];
  displayedColumns: string[] = Constants.ORDER_BOOK_COLUMNS;
  @Input() currencyPair!: string;

  ngOnInit(): void {
    this.binanceService.getOrderBookData(this.currencyPair).subscribe(data => {
      this.bids = data.b.map(([price, amount]: [number, number]) => ({ price, amount })).slice(0, 10);
      this.asks = data.a.map(([price, amount]: [number, number]) => ({ price, amount })).slice(0, 10);
    })
  }
}
