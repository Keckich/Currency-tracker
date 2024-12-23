import { ɵ$localize } from '@angular/localize';

import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";
import { StateMessages } from '../../../shared/constants.value';
import { format } from '../../../shared/utilities';

export function takeProfitValidator(currentPrice: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const enteredValue = control.value;
    if (enteredValue != null && enteredValue < currentPrice) {
      return {
        takeProfitInvalid: format(StateMessages.ERROR_PROFIT_MORE_THAN, { currentPrice: currentPrice }),
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
        stopLossInvalid: format(StateMessages.ERROR_LOSS_LESS_THAN, { currentPrice: currentPrice }),
      }
    }
    return null;
  }
}
