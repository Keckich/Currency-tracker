import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { tradeAnalyticsGuard } from './trade-analytics.guard';

describe('tradeAnalyticsGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => tradeAnalyticsGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});
