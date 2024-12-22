import { ɵ$localize } from '@angular/localize';
import { format } from '../utilities';
import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";
import { StateMessages } from '../constants.value';

export class NumberValidators {
  static moreThan(value: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const enteredValue = control.value;
      if (enteredValue != null && enteredValue <= value) {
        return {
          moreThanInvalid: format(StateMessages.ERROR_MORE_THAN, { value: value })
        }
      }
      return null;
    }
  }

  static lessThan(value: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const enteredValue = control.value;
      if (enteredValue != null && enteredValue >= value) {
        return {
          lessThanInvalid: format(StateMessages.ERROR_LESS_THAN, { value: value })
        }
      }
      return null;
    }
  }
}
