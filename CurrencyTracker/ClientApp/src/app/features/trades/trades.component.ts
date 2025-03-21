import { AfterViewInit, ChangeDetectorRef, Component, inject, Input, OnInit, ViewChild } from '@angular/core';
import { Trade } from '../../shared/shared.model';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { Constants } from '../../shared/constants.value';
import { TradesService } from '../../core/services/trades.service';
import { CommonModule } from '@angular/common';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { Routes } from '../../shared/constants.value';
import {RouterLink} from "@angular/router";

@Component({
  selector: 'app-trades',
  standalone: true,
  imports: [
    MatTableModule,
    MatSortModule,
    CommonModule,
    MatPaginator,
    RouterLink,
  ],
  templateUrl: './trades.component.html',
  styleUrl: './trades.component.css'
})
export class TradesComponent implements OnInit {
  private tradesService = inject(TradesService);

  @Input() trades: Trade[] = [];
  displayedColumns = Constants.TRADE_COLUMNS;
  dataSource = new MatTableDataSource<Trade>();

  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  length: number = 0;
  pageIndex: number = 0;
  pageSize: number = 10;
  Routes = Routes;

  ngOnInit(): void {
    this.fetchTrades(this.pageIndex, this.pageSize);
    this.sort.sort({ id: 'date', start: 'desc', disableClear: true });
  }

  handlePageEvent(e: PageEvent): void {
    this.tradesService.getPaginatedTrades(e.pageIndex, e.pageSize).subscribe();
  }

  fetchTrades(page: number, pageSize: number): void {
    this.tradesService.trades$.subscribe({
      next: trades => {
        this.dataSource.data = trades.data;
        this.dataSource.sort = this.sort;
        this.length = trades.totalItems;
      },
    });

    this.tradesService.getPaginatedTrades(page, pageSize).subscribe();
  }
}
