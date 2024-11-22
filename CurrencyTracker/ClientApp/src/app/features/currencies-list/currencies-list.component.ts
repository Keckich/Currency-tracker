import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { BinanceService } from '../../core/services/binance.service';
import { Currency, SortOrder } from '../../shared/shared.model';
import { SortPipe } from '../../shared/pipes/sort.pipe';
import { SearchControlComponent } from '../../shared/components/search-control/search-control.component';

import { MatList, MatListItem } from '@angular/material/list';
import { Routes } from '../../shared/constants.value';

@Component({
  selector: 'app-currencies-list',
  standalone: true,
  imports: [
    MatList,
    MatListItem,
    RouterLink,
    SortPipe,
    SearchControlComponent,
  ],
  templateUrl: './currencies-list.component.html',
  styleUrl: './currencies-list.component.css'
})
export class CurrenciesListComponent implements OnInit {
  private binanceService = inject(BinanceService);

  Routes = Routes;
  currenciesList: Currency[] = [];
  filteredList: Currency[] = [];
  searchQuery: string = '';
  sortOrder: SortOrder = SortOrder.Asc;

  ngOnInit(): void {
    this.loadCurrenciesList();
  }

  loadCurrenciesList(): void {
    this.binanceService.getCryptocurrencies().subscribe({
      next: data => {
        this.currenciesList = data;
        this.filteredList = data;
      }
    });
  }

  handleSearchEvent(currencies: Currency[]): void {
    this.filteredList = currencies;
  }
}
