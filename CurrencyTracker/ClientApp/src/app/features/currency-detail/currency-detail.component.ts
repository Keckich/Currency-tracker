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

@Component({
  selector: 'app-currency-detail',
  standalone: true,
  imports: [
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

  trades!: Trade[];
  currencyPair: string = '';
  selectedInterval: ChartInterval = ChartInterval.S1;

  ngOnInit(): void {
    this.currencyPair = this.loadCurrency();
    this.predictionService.getHammerPrediction(this.currencyPair, this.selectedInterval).subscribe(data => {
      console.log(data)
    });
  }

  loadCurrency(): string {
    const id = this.routeService.getParams(this.activatedRoute.snapshot.paramMap).id
    return id || '';
  }

  handleBuy(trades: Trade[]): void {
    this.trades = trades;
  }

  onIntervalChange(interval: ChartInterval): void {
    this.selectedInterval = interval;
  }
}
