import { Component, Input, OnDestroy, OnInit, ViewChild, inject } from '@angular/core';
import { BinanceService } from '../../core/services/binance.service';
import { Observable, Subscription } from 'rxjs';
import { Trade, Transaction } from '../../shared/shared.model';
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
  private currentTrade: Trade = {} as Trade;
  private formBuilder = inject(FormBuilder);

  price$: Observable<any> = new Observable();
  transactionForm = this.formBuilder.group<Transaction>({
    amount: [0, [Validators.required, NumberValidators.moreThan(0)]]
  })
  isLimitSectionOpened: boolean = false;
  isLimitOrderValid: boolean = false;

  @Input() currencyPair: string = '';
  @ViewChild(LimitOrderComponent) limitOrderComponent!: LimitOrderComponent;

  ngOnInit(): void {
    //this.priceSubscription = this.getPrice();
    this.getPrice();
  }

  getPrice(): void {
    this.binanceService.getCryptoPriceUpdates(this.currencyPair).subscribe(data => {
      this.binanceService.onPriceUpdates(rawData => {
        const data = JSON.parse(rawData);

        if (data.s === this.currencyPair) {
          const currentDate = new Date();
          const price = parseFloat(data.c);
          this.tradesService.updatePrices({ [this.currencyPair]: price });
          this.setCurrentTradeValue(currentDate, price);
        }
      });
    });

    /*this.price$.subscribe({
      next: () => {
        console.log(`Subscribed to ${this.currencyPair} price updates`)
      },
      error: error => console.log(`Error occured: ${error}`)
    })*/
  }

  private setCurrentTradeValue(currentDate: Date, price: number): void {
    this.currentTrade = {} as Trade;
    this.currentTrade.price = price;
    this.currentTrade.date = currentDate;
    this.currentTrade.currency = this.currencyPair;
  }

  buy(): void {
    if (!this.transactionForm.valid) {
      this.showErrors();
    }
    if (this.isLimitSectionOpened && !this.isLimitOrderValid) {
      this.limitOrderComponent.showErrors();
    }
    if (this.canBuy()) {
      this.currentTrade.amount = this.transactionForm.controls.amount.value ?? 0;
      this.currentTrade.value = (this.currentTrade.price ?? 0) * this.currentTrade.amount;
      this.tradesService.addTrade(this.currentTrade);
    }
  }

  private canBuy(): boolean {
    return (!this.isLimitSectionOpened || (this.isLimitSectionOpened && this.isLimitOrderValid)) && this.transactionForm.valid;
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
      //this.priceSubscription.unsubscribe();
    }
  }
}
