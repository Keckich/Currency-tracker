import { CanActivateFn, Router } from '@angular/router';
import { TradesService } from '../services/trades.service';
import { inject } from '@angular/core';
import { map } from 'rxjs';
import { StateMessages } from '../../shared/constants.value';

export const tradeAnalyticsGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const tradesService = inject(TradesService);

  return tradesService.getTrades().pipe(
    map(trades => {
      if (trades.length > 0) {
        return true;
      } else {
        router.navigate(['/access-denied'], {
          state: { message: StateMessages.ERROR_ACCESS_ANALYTICS_GUARD },
        });
        return false;
      }
    })
  );
};
