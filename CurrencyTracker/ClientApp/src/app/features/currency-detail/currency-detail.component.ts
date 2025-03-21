import { Component, OnInit, inject } from '@angular/core';
import { Trade } from '../../shared/shared.model';
import { ChartComponent } from '../chart/chart.component';
import { TradesComponent } from '../trades/trades.component';
import { RouteService } from '../../core/services/route.service';
import { OrderBookComponent } from '../order-book/order-book.component';
import { ChartInterval } from '../../shared/shared.enum';
import { filter } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-currency-detail',
  standalone: true,
  imports: [
    CommonModule,
    ChartComponent,
    TradesComponent,
    OrderBookComponent,
  ],
  templateUrl: './currency-detail.component.html',
  styleUrl: './currency-detail.component.css'
})
export class CurrencyDetailComponent implements OnInit {
  private routeService = inject(RouteService);

  trades!: Trade[];
  currencyPair!: string;
  selectedInterval: ChartInterval = ChartInterval.S1;

  ngOnInit(): void {
    this.routeService.params$
      .pipe(filter(params => params?.id != null))
      .subscribe(params => this.currencyPair = params!.id!);
  }

  handleBuy(trades: Trade[]): void {
    this.trades = trades;
  }

  onIntervalChange(interval: ChartInterval): void {
    this.selectedInterval = interval;
  }
}
