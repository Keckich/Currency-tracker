import { Component, OnInit, inject } from '@angular/core';
import { Trade } from '../../shared/shared.model';
import { Constants } from '../../shared/constants.value';
import { ChartComponent } from '../chart/chart.component';
import { TradesComponent } from '../trades/trades.component';
import { ActivatedRoute } from '@angular/router';
import { RouteService } from '../../core/services/route.service';
import { OrderBookService } from '../../core/services/order-book.service';
import { OrderBookComponent } from '../order-book/order-book.component';

@Component({
  selector: 'app-currency-detail',
  standalone: true,
  imports: [
    ChartComponent,
    TradesComponent,
    OrderBookComponent,
  ],
  templateUrl: './currency-detail.component.html',
  styleUrl: './currency-detail.component.css'
})
export class CurrencyDetailComponent implements OnInit {
  private activatedRoute = inject(ActivatedRoute);
  private routeService = inject(RouteService);
  private orderBookService = inject(OrderBookService);

  trades!: Trade[];
  currencyPair: string = '';

  ngOnInit(): void {
    this.currencyPair = this.loadCurrency();
  }

  loadCurrency(): string {
    const id = this.routeService.getParams(this.activatedRoute.snapshot.paramMap).id
    return id || '';
  }

  handleBuy(trades: Trade[]): void {
    this.trades = trades;
  }
}
