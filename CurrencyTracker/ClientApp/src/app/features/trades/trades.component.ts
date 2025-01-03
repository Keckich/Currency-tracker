import { AfterViewInit, ChangeDetectorRef, Component, inject, Input, OnInit, ViewChild } from '@angular/core';
import { Trade } from '../../shared/shared.model';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { Constants } from '../../shared/constants.value';
import { TradesService } from '../../core/services/trades.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-trades',
  standalone: true,
  imports: [
    MatTableModule,
    MatSortModule,
    CommonModule,
  ],
  templateUrl: './trades.component.html',
  styleUrl: './trades.component.css'
})
export class TradesComponent implements OnInit {
  private tradesService = inject(TradesService);
  private cdr = inject(ChangeDetectorRef);
  @Input() trades: Trade[] = [];
  displayedColumns = Constants.TRADE_COLUMNS;
  dataSource = new MatTableDataSource<Trade>();

  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit(): void {
    this.tradesService.trades$.subscribe({
      next: trades => {
        this.dataSource.data = trades;
        this.dataSource.sort = this.sort;
        this.sort.sort({ id: 'date', start: 'desc', disableClear: true });
      },
    });

    this.tradesService.getTrades().subscribe();
  }
}
