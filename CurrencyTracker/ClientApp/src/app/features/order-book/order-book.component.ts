import { Component, Input, OnInit, inject } from '@angular/core';
import { BinanceService } from '../../core/services/binance.service';
import { MatTableModule } from '@angular/material/table';
import { MatDividerModule } from '@angular/material/divider';
import { Constants } from '../../shared/constants.value';
import { CommonModule } from '@angular/common';
import { Order } from '../../shared/shared.model';

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

  bids: Order[] = [];
  asks: Order[] = [];
  displayedColumns: string[] = Constants.ORDER_BOOK_COLUMNS;
  @Input() currencyPair!: string;

  ngOnInit(): void {
    this.binanceService.getOrderBookData(this.currencyPair).subscribe(data => {
      this.bids = data.bids;
      this.asks = data.asks;
    })
  }
}
