import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
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
export class TradesComponent implements OnInit, AfterViewInit {
  @Input() trades: Partial<Trade>[] = [];
  displayedColumns = Constants.DISPLAYED_COLUMNS;
  dataSource = new MatTableDataSource<Partial<Trade>>();

  @ViewChild(MatSort) sort!: MatSort;

  constructor(private tradesService: TradesService) { }

  ngOnInit(): void {
    this.tradesService.trades$.subscribe({
      next: trades => {
        this.dataSource.data = trades.map((trade, index) => ({
          ...trade,
          position: index + 1,
        }));
      },
    });
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
    this.sort.sort({ id: 'position', start: 'desc', disableClear: true });
  }
}
