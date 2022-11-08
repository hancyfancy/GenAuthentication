import { Directive } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, Validator } from '@angular/forms';

@Directive({
  selector: '[lengthValidator]',
  providers: [{
    provide: NG_VALIDATORS,
    useExisting: LengthValidatorDirective,
    multi: true
  }]
})
export class LengthValidatorDirective implements Validator {

  constructor() { }

  validate(control: AbstractControl): { [key: string]: any } | null {
    if (control.value && control.value.length > 100) {
      return { 'lengthInvalid': true };
    }
    return null;
  }
}
