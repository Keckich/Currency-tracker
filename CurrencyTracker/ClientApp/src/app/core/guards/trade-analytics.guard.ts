import { CanActivateFn, Router } from '@angular/router';
import { TradesService } from '../services/trades.service';
import { inject } from '@angular/core';

export const tradeAnalyticsGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const tradesService = inject(TradesService);

  if (tradesService.getTradesValue().length > 0) {
    return true;
  }

  router.navigate(['/access-denied'], {
    state: { message: $localize`:@@accessDeniedPageMessage:You don't have access to this page. Please, made at least one trade operation.` },
  });

  return false
};
