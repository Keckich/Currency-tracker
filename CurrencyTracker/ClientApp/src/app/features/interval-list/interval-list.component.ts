import { Component, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
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
export class IntervalListComponent<T extends string | number | symbol> implements OnInit {
  @Input() chartIntervals!: Partial<Record<T, string>>;
  @Output() intervalChange = new EventEmitter<T>();
  @Input() selectedInterval!: T;
  intervals: { key: T; value: string | number }[] = [];

  ngOnInit(): void {
    this.intervals = Object.entries(this.chartIntervals).map(([key, value]) => ({
      key: key as T,
      value: value as string | number
    }));
  }

  onIntervalSelect(interval: T): void {
    this.selectedInterval = interval;
    this.intervalChange.emit(this.selectedInterval);
  }
}
