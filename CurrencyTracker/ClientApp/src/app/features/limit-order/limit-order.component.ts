import { Component, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { FormGroup, FormControl, FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatError, MatFormField, MatInput, MatLabel } from '@angular/material/input';
import { Observable, Subscription } from 'rxjs';
import { stopLossValidator, takeProfitValidator } from './validators/limit-valiators';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-limit-order',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    MatButton,
    MatInput,
    MatLabel,
    MatFormField,
    MatError,
    CommonModule
  ],
  templateUrl: './limit-order.component.html',
  styleUrl: './limit-order.component.css'
})
export class LimitOrderComponent implements OnInit {
  private formBuilder = inject(FormBuilder);

  @Output() isValidChange = new EventEmitter<boolean>();
  @Input() price$!: Observable<any>;

  limitsForm = this.formBuilder.group({
    takeProfit: [0],
    stopLoss: [0],
  })

  ngOnInit(): void {
    this.limitsForm.statusChanges.subscribe(() => {
      this.isValidChange.emit(this.limitsForm.valid);
    })

    this.price$.subscribe({
      next: data => {
        const price = parseFloat(data.c);
        this.limitsForm.get('takeProfit')?.setValidators([
          Validators.required,
          takeProfitValidator(price),
        ]);

        this.limitsForm.get('stopLoss')?.setValidators([
          Validators.required,
          stopLossValidator(price),
        ]);

        this.limitsForm.get('takeProfit')?.updateValueAndValidity();
        this.limitsForm.get('stopLoss')?.updateValueAndValidity();
      }
    })
  }

  showErrors(): void {
    Object.keys(this.limitsForm.controls).forEach(controlName => {
      const control = this.limitsForm.get(controlName);
      control?.markAsTouched();
      control?.updateValueAndValidity();
    })
  }
}
