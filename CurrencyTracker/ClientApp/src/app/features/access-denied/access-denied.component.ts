import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-access-denied',
  standalone: true,
  imports: [],
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
