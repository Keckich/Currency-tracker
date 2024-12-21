import { Component, EventEmitter, OnInit, Output, inject } from '@angular/core';
import { ChartIntervals } from '../../shared/constants.value';
import { CommonModule } from '@angular/common';
import { MatList, MatListItem } from '@angular/material/list';
import { ChartInterval } from '../../shared/shared.enum';

@Component({
  selector: 'app-interval-list',
  standalone: true,
  imports: [
    CommonModule,
  ],
  templateUrl: './interval-list.component.html',
  styleUrl: './interval-list.component.css'
})
export class IntervalListComponent {
  @Output() intervalChange = new EventEmitter<ChartInterval>();
  intervals: { key: ChartInterval; value: string }[] = Object.entries(ChartIntervals).map(([key, value]) => ({
    key: key as ChartInterval,
    value
  }));
  selectedInterval: ChartInterval = ChartInterval.S1;

  onIntervalSelect(interval: ChartInterval): void {
    this.selectedInterval = interval;
    this.intervalChange.emit(this.selectedInterval);
  }
}
