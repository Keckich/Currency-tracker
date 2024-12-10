import { AnalysisResult, Trade } from "./shared.model";
import { ApexChart, ApexPlotOptions, ApexTitleSubtitle, ApexXAxis, ApexYAxis } from 'ng-apexcharts';

import { nameof } from 'ts-simple-nameof';

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

export const ChartIntervals = {
  S1: { value: '1s', display: $localize`:@@chartIntervalS1:1s`},
  M1: { value: '1m', display: $localize`:@@chartIntervalM1:1m`},
  M15: { value: '15m', display: $localize`:@@chartIntervalM15:15m`},
  H1: { value: '1h', display: $localize`:@@chartIntervalH1:1h`},
  H4: { value: '4h', display: $localize`:@@chartIntervalH4:4h`},
  H12: { value: '12h', display: $localize`:@@chartIntervalH12:12h`},
  D1: { value: '1d', display: $localize`:@@chartIntervalD1:1d`},
}

export const AnalysisRecommenations = {
  SELL_PROFIT: $localize`:@@sellProfitRecommendation:Sell to lock in a profit.`,
  KEEP_GROWTH: $localize`:@@keepGrowthRecommendation:Keep holding, further growth is possible.`,
  SELL_LOSS: $localize`:@@sellLossRecommendation:Sell to minimize losses.`,
  KEEP_RECOVER: $localize`:@@keepRecoverRecommendation:Keep it if you expect the price to recover.`,
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
