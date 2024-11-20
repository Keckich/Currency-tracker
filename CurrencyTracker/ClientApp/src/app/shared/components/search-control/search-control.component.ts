import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Currency } from '../../shared.model';
import { Subscription, debounceTime, distinctUntilChanged } from 'rxjs';

import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-search-control',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    MatInput,
    MatIcon,
    MatFormField,
  ],
  templateUrl: './search-control.component.html',
  styleUrl: './search-control.component.css'
})
export class SearchControlComponent implements OnInit, OnDestroy {
  searchControl = new FormControl('');
  valueChangesSubscription!: Subscription;
  @Input() currencies: Currency[] = [];
  @Output() filteredEvent = new EventEmitter<Currency[]>;

  ngOnInit(): void {
    this.valueChangesSubscription = this.searchControl.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged()
      )
      .subscribe(searchText => {
        this.filteredEvent.emit(this.currencies.filter(c => searchText ? c.symbol.toLowerCase().includes(searchText.toLowerCase()) : c.symbol))
      })
  }

  ngOnDestroy(): void {
    if (this.valueChangesSubscription) {
      this.valueChangesSubscription.unsubscribe();
    }
  }
}
