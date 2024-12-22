import { AnalysisResult, Trade } from "./shared.model";
import { ApexChart, ApexPlotOptions, ApexTitleSubtitle, ApexXAxis, ApexYAxis } from 'ng-apexcharts';

import { nameof } from 'ts-simple-nameof';
import { ChartInterval } from "./shared.enum";

export const Constants = {
  CURRENCIES: ['BTCUSDT', 'ETHUSDT', 'BNBUSDT'],
  TRADE_COLUMNS: [
    nameof<Trade>(t => t.position),
    nameof<Trade>(t => t.price),
    nameof<Trade>(t => t.amount),
    nameof<Trade>(t => t.value),
    nameof<Trade>(t => t.date),
    nameof<Trade>(t => t.currency),
    nameof<Trade>(t => t.takeProfit),
    nameof<Trade>(t => t.stopLoss)
  ] as (keyof Trade)[],
  ANALYSIS_COLUMNS: [
    nameof<AnalysisResult>(t => t.position),
    nameof<AnalysisResult>(t => t.currency),
    nameof<AnalysisResult>(t => t.avgPrice),
    nameof<AnalysisResult>(t => t.tradeInfo.totalAmount),
    nameof<AnalysisResult>(t => t.roi),
    nameof<AnalysisResult>(t => t.recommendation),
  ] as (keyof AnalysisResult)[],
  CANDLE_COLORS: { green: '#00B746', red: '#EF403C' },
  CANDLE_CHART_SIZE: 100,
}

export const Routes = {
  CURRENCY: 'currency',
  ANALYTICS: 'analytics',
  ACCESS_DENIED: 'access-denied'
}

export const ChartIntervals: Partial<Record<ChartInterval, string>> = {
  [ChartInterval.S1]: $localize`:@@chartIntervalS1:1s`,
  [ChartInterval.M1]: $localize`:@@chartIntervalS1:1m`,
  [ChartInterval.M15]: $localize`:@@chartIntervalS1:15m`,
  [ChartInterval.H1]: $localize`:@@chartIntervalS1:1h`,
  [ChartInterval.H4]: $localize`:@@chartIntervalS1:4h`,
  [ChartInterval.H12]: $localize`:@@chartIntervalS1:12h`,
  [ChartInterval.D1]: $localize`:@@chartIntervalS1:1d`,
}

export const AnalysisRecommenations = {
  SELL_PROFIT: $localize`:@@sellProfitRecommendation:Sell to lock in a profit.`,
  KEEP_GROWTH: $localize`:@@keepGrowthRecommendation:Keep holding, further growth is possible.`,
  SELL_LOSS: $localize`:@@sellLossRecommendation:Sell to minimize losses.`,
  KEEP_RECOVER: $localize`:@@keepRecoverRecommendation:Keep it if you expect the price to recover.`,
}

export const StateMessages = {
  ERROR_MORE_THAN: $localize`:@@errorMoreThan:Entered value should be more than {value}`,
  ERROR_LESS_THAN: $localize`:@@errorLessThan:Entered value should be less than {value}`,
}

export const ChartOptions: {
  chart: ApexChart,
  xaxis: ApexXAxis,
  yaxis: ApexYAxis,
  title: ApexTitleSubtitle,
  plotOptions: ApexPlotOptions,
} = {
  chart: {
    type: 'candlestick',
    height: 350,
    animations: {
      enabled: false,
    },
    offsetX: 0,
  },
  xaxis: {
    type: 'datetime',
    tickPlacement: 'on',
  },
  yaxis: {
    tooltip: {
      enabled: true,
    },
  },
  title: {
    text: '',
  },
  plotOptions: {
    candlestick: {
      colors: {
        upward: Constants.CANDLE_COLORS.green,
        downward: Constants.CANDLE_COLORS.red,
      },
    }
  }
}
