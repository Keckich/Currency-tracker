import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransactionComponent } from './transaction.component';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { By } from '@angular/platform-browser';
import { StateMessages } from '../../shared/constants.value';
import { inject } from '@angular/core';
import { Trade, Transaction } from '../../shared/shared.model';
import { format } from '../../shared/utilities';
import { TradesService } from '../../core/services/trades.service';

describe('TransactionComponent', () => {
  let component: TransactionComponent;
  let fixture: ComponentFixture<TransactionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TransactionComponent, BrowserAnimationsModule],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(TransactionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show an error message for invalid input', () => {
    const input = fixture.debugElement.query(By.css('input')).nativeElement;
    input.value = -5;
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    const control = component.transactionForm.get('amount');
    control?.setErrors({ moreThan: +input.value });
    control?.markAsTouched();
    control?.updateValueAndValidity();
    fixture.detectChanges();

    const error = fixture.debugElement.query(By.css('mat-error'));
    expect(error.nativeElement.textContent).toContain(format(StateMessages.ERROR_MORE_THAN, { value: 0 }));
  });

  it('should call the service to execute the transaction', () => {
    const mockService = TestBed.inject(TradesService);
    spyOn(mockService, 'addTrade');

    const input = fixture.debugElement.query(By.css('input')).nativeElement;
    const button = fixture.debugElement.query(By.css('button')).nativeElement;

    input.value = 10;
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    button.click();
    var trade: Partial<Trade> = {
      amount: +input.value,
      value: 0
    }
    component.buy();
    expect(mockService.addTrade).toHaveBeenCalledWith(jasmine.objectContaining(trade));
  });
});
