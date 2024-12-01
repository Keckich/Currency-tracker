import { Component, EventEmitter, OnInit, Output, inject } from '@angular/core';
import { ChartIntervals } from '../../shared/constants.value';
import { CommonModule } from '@angular/common';
import { MatList, MatListItem } from '@angular/material/list';

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
  @Output() intervalChange = new EventEmitter<string>();
  intervals = Object.entries(ChartIntervals).map(([key, { value, display }]) => ({ key, value, display }));
  selectedInterval: string = ChartIntervals.S1.value;

  onIntervalSelect(interval: string): void {
    this.selectedInterval = interval;
    this.intervalChange.emit(this.selectedInterval);
  }
}
