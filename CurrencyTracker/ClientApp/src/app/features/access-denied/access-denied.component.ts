import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatCard, MatCardActions, MatCardContent, MatCardTitle, MatCardHeader } from '@angular/material/card';

@Component({
  selector: 'app-access-denied',
  standalone: true,
  imports: [
    MatCard,
    MatCardActions,
    MatCardContent,
    MatCardHeader,
    MatCardTitle,
    MatButton,
    MatIcon,
  ],
  templateUrl: './access-denied.component.html',
  styleUrl: './access-denied.component.css'
})
export class AccessDeniedComponent {
  private router = inject(Router);
  message: string = '';

  constructor() {
    const navigtion = this.router.getCurrentNavigation();
    this.message = navigtion?.extras.state?.['message'] || 'Access denied. Please, contact administrator.'
  }
}
