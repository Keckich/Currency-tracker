import { Component, Input, OnDestroy, OnInit, ViewChild, inject } from '@angular/core';
import { BinanceService } from '../../core/services/binance.service';
import { Observable, Subscription } from 'rxjs';
import { Trade } from '../../shared/shared.model';
import { LimitOrderComponent } from '../limit-order/limit-order.component';
import { TradesService } from '../../core/services/trades.service';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { MatButton } from '@angular/material/button';
import { MatError, MatFormField, MatInput, MatLabel } from '@angular/material/input';
import { MatIcon } from '@angular/material/icon';
import { MatCheckbox } from '@angular/material/checkbox';
import { NumberValidators } from '../../shared/validators/number-validators';

@Component({
  selector: 'app-transaction',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    CommonModule,
    MatButton,
    MatInput,
    MatLabel,
    MatIcon,
    MatFormField,
    MatCheckbox,
    MatError,
    LimitOrderComponent,
  ],
  templateUrl: './transaction.component.html',
  styleUrl: './transaction.component.css'
})
export class TransactionComponent implements OnInit, OnDestroy {
  private binanceService = inject(BinanceService);
  private tradesService = inject(TradesService);
  private priceSubscription: Subscription = new Subscription;
  private currentTrade!: Partial<Trade>;
  private formBuilder = inject(FormBuilder);

  price$: Observable<any> = new Observable();
  transactionForm = this.formBuilder.group({
    amount: [0, [Validators.required, NumberValidators.moreThan(0)]]
  })
  isLimitSectionOpened: boolean = false;
  isLimitOrderValid: boolean = false;

  @Input() currencyPair: string = '';
  @ViewChild(LimitOrderComponent) limitOrderComponent!: LimitOrderComponent;

  ngOnInit(): void {
    this.priceSubscription = this.getPrice();
  }

  getPrice(): Subscription {
    this.price$ = this.binanceService.getCryptoPriceUpdates(this.currencyPair);

    return this.price$.subscribe({
      next: data => {
        const currentDate = new Date();
        const price = parseFloat(data.c);
        this.tradesService.updatePrices({ [this.currencyPair]: price });
        this.setCurrentTradeValue(currentDate, price);
      },
      error: error => console.log(`Error occured: ${error}`)
    })
  }

  private setCurrentTradeValue(currentDate: Date, price: number): void {
    this.currentTrade = {
      price: price,
      date: currentDate,
      currency: this.currencyPair,
    };
  }

  buy(): void {
    if (!this.transactionForm.valid) {
      this.showErrors();
    }
    if (this.isLimitSectionOpened && !this.isLimitOrderValid) {
      this.limitOrderComponent.showErrors();
    }
    if ((!this.isLimitSectionOpened || (this.isLimitSectionOpened && this.isLimitOrderValid)) && this.transactionForm.valid) {
      this.currentTrade.amount = this.transactionForm.get('amount')?.value ?? 0;
      this.currentTrade.value = (this.currentTrade.price ?? 0) * this.currentTrade.amount;
      this.tradesService.addTrade(this.currentTrade);
    }
  }

  showErrors(): void {
    Object.keys(this.transactionForm.controls).forEach(controlName => {
      const control = this.transactionForm.get(controlName);
      control?.markAsTouched();
      control?.updateValueAndValidity();
    })
  }

  onLimitOrderValidityChange(isValid: boolean): void {
    this.isLimitOrderValid = isValid;
  }

  ngOnDestroy(): void {
    if (this.priceSubscription) {
      this.priceSubscription.unsubscribe();
    }
  }
}
