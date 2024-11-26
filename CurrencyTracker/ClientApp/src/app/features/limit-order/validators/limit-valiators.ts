import { ɵ$localize } from '@angular/localize';

import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function takeProfitValidator(currentPrice: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const enteredValue = control.value;
    if (enteredValue != null && enteredValue < currentPrice) {
      return {
        takeProfitInvalid: $localize`:@@errorTakeProfit:Take-profit should be more than ${currentPrice}`
      }
    }
    return null;
  }
}

export function stopLossValidator(currentPrice: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const enteredValue = control.value;
    if (enteredValue != null && enteredValue > currentPrice) {
      return {
        stopLossInvalid: $localize`:@@errorStopLoss:Stop-loss should be less than ${currentPrice}`
      }
    }
    return null;
  }
}
