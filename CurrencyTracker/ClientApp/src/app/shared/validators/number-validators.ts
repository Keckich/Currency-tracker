import { ɵ$localize } from '@angular/localize';

import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export class NumberValidators {
  static moreThan(value: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const enteredValue = control.value;
      if (enteredValue != null && enteredValue <= value) {
        return {
          moreThanInvalid: $localize`:@@errorMoreThan:Entered value should be more than ${value}`
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
          lessThanInvalid: $localize`:@@errorLessThan:Entered value should be less than ${value}`
        }
      }
      return null;
    }
  }
}
