import { Component, OnInit, inject } from '@angular/core';
import { Trade } from '../../shared/shared.model';
import { Constants } from '../../shared/constants.value';
import { ChartComponent } from '../chart/chart.component';
import { TradesComponent } from '../trades/trades.component';
import { ActivatedRoute } from '@angular/router';
import { RouteService } from '../../core/services/route.service';
import { OrderBookComponent } from '../order-book/order-book.component';
import { PredictionService } from '../../core/services/prediction.service';
import { ChartInterval } from '../../shared/shared.enum';
import { TradeSignalComponent } from '../trade-signal/trade-signal.component';
import { BehaviorSubject, Observable, filter, map } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-currency-detail',
  standalone: true,
  imports: [
    CommonModule,
    ChartComponent,
    TradesComponent,
    OrderBookComponent,
    TradeSignalComponent,
  ],
  templateUrl: './currency-detail.component.html',
  styleUrl: './currency-detail.component.css'
})
export class CurrencyDetailComponent implements OnInit {
  private activatedRoute = inject(ActivatedRoute);
  private routeService = inject(RouteService);
  private predictionService = inject(PredictionService);
  private router = inject(Router);

  trades!: Trade[];
  currencyPair!: string;
  selectedInterval: ChartInterval = ChartInterval.S1;

  ngOnInit(): void {
    this.loadCurrency();
  }
  loadCurrency(): void {
    this.activatedRoute.paramMap.subscribe(params => {
      this.currencyPair = this.routeService.getParams(params).id || '';
    });
  }

  handleBuy(trades: Trade[]): void {
    this.trades = trades;
  }

  onIntervalChange(interval: ChartInterval): void {
    this.selectedInterval = interval;
  }
}
