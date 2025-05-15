import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { NgApexchartsModule } from 'ng-apexcharts';
import { ChartComponent } from './features/chart/chart.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { TradesComponent } from './features/trades/trades.component';
import { CurrencyDetailComponent } from './features/currency-detail/currency-detail.component';
import { Routes } from './shared/constants.value';
import { TradeAnalyticsComponent } from './features/trade-analytics/trade-analytics.component';
import { tradeAnalyticsGuard } from './core/guards/trade-analytics.guard';
import { AccessDeniedComponent } from './features/access-denied/access-denied.component';
import { BacktestComponent } from './features/backtest/backtest.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ApiAuthorizationModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: `${Routes.CURRENCY}/:id`, component: CurrencyDetailComponent },
      { path: `${Routes.ANALYTICS}`, component: TradeAnalyticsComponent, canActivate: [tradeAnalyticsGuard] },
      { path: `${Routes.ACCESS_DENIED}`, component: AccessDeniedComponent },
      { path: `${Routes.BACKTEST}`, component: BacktestComponent },
    ])
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true },
    provideAnimationsAsync()
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
