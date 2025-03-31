import { AnalysisResult, Trade } from "./shared.model";
import { ApexChart, ApexFill, ApexMarkers, ApexPlotOptions, ApexStroke, ApexTitleSubtitle, ApexTooltip, ApexXAxis, ApexYAxis } from 'ng-apexcharts';

import { nameof } from 'ts-simple-nameof';
import { ChartInterval, PnLInterval, TradeSignalType } from "./shared.enum";
import { inject } from "@angular/core";

export const Constants = {
  CURRENCIES: ['BTCUSDT', 'ETHUSDT', 'BNBUSDT'],
  TRADE_COLUMNS: [
    nameof<Trade>(t => t.id),
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
  ORDER_BOOK_COLUMNS: [
    'Price',
    'Amount',
  ],
  CHART_COLORS: { green: '#00B746', red: '#EF403C', blue: '#00BFFF', white: '#fff' },
  CANDLE_CHART_SIZE: 100,
  PNL_CHART_TITLE: 'PnL Data',
  PNL_CHART_Y_TEXT: 'Balance (USD)',
  WAITING_SIGNAL: 'Waiting for signal...',
  CONNECTION_LOST_SIGNAL: 'Connection lost!',
  TRADE_SIGNAL: 'Sell/Buy signal: {0} with {1}% confidence',
}

export const Routes = {
  CURRENCY: 'currency',
  ANALYTICS: 'analytics',
  ACCESS_DENIED: 'access-denied'
}

export const ApiUrls = {
  TRADES: 'trades',
  ORDER_BOOK: 'orderbook',
  PATTERN_ANALYZER: 'patternanalyzer',
  BINANCE: 'binance',
  TRADE_SIGNAL: 'tradesignal',
}

export const ApiUrlResources = {
  PAGINATED: 'paginated',
  PNL: 'pnl',
  SUBSCRIBE: 'subscribe',
  UNSUBSCRIBE: 'unsubscribe',
}

export const ChartIntervals: Record<ChartInterval, string> = {
  [ChartInterval.S1]: $localize`:@@chartIntervalS1:1s`,
  [ChartInterval.M1]: $localize`:@@chartIntervalS1:1m`,
  [ChartInterval.M15]: $localize`:@@chartIntervalS1:15m`,
  [ChartInterval.H1]: $localize`:@@chartIntervalS1:1h`,
  [ChartInterval.H4]: $localize`:@@chartIntervalS1:4h`,
  [ChartInterval.H12]: $localize`:@@chartIntervalS1:12h`,
  [ChartInterval.D1]: $localize`:@@chartIntervalS1:1d`,
}

export const PnLIntervals: Record<PnLInterval, string> = {
  [PnLInterval.D1]: $localize`:@@pnlIntervalD1:1d`,
  [PnLInterval.D7]: $localize`:@@pnlIntervalD7:7d`,
  [PnLInterval.D14]: $localize`:@@pnlIntervalD14:14d`,
  [PnLInterval.D21]: $localize`:@@pnlIntervalD21:21d`,
  [PnLInterval.D30]: $localize`:@@pnlIntervalD30:30d`,
}

export const TradeSignals: Record<number, string> = {
  [TradeSignalType.Buy]: $localize`:@@signalBuy:Buy`,
  [TradeSignalType.Sell]: $localize`:@@signalSell:Sell`,
  [TradeSignalType.Neutral]: $localize`:@@signalNeutral:Neutral`,
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
  ERROR_PROFIT_MORE_THAN: $localize`:@@errorTakeProfit:Take-profit should be more than {currentPrice}`,
  ERROR_LOSS_LESS_THAN: $localize`:@@errorStopLoss:Stop-loss should be less than {currentPrice}`,
  ERROR_ACCESS_ANALYTICS_GUARD: $localize`:@@accessDeniedPageMessage:You don't have access to this page. Please, made at least one trade operation.`,
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
        upward: Constants.CHART_COLORS.green,
        downward: Constants.CHART_COLORS.red,
      },
    }
  }
}

export const PnLChartOptions: {
  chart: ApexChart,
  stroke: ApexStroke,
  markers: ApexMarkers,
  fill: ApexFill,
  title: ApexTitleSubtitle,
  tooltip: ApexTooltip,
  xaxis: ApexXAxis,
  yaxis: ApexYAxis,
} = {
  chart: {
    type: "line",
    height: 350,
    width: '50%',
  },
  stroke: {
    curve: 'smooth',
    width: 3,
    colors: [Constants.CHART_COLORS.blue]
  },
  markers: {
    size: 5,
    colors: [Constants.CHART_COLORS.blue],
    strokeColors: Constants.CHART_COLORS.white,
    strokeWidth: 2
  },
  fill: {
    type: 'solid',
    opacity: 0.2
  },
  title: {
    text: Constants.PNL_CHART_TITLE,
  },
  tooltip: {
  },
  xaxis: {
    type: "datetime",
    labels: {
      format: "dd MMM yyyy"
    }
  },
  yaxis: {
    title: {
      text: Constants.PNL_CHART_Y_TEXT,
    },
    labels: {
      formatter: (value: number) => value.toFixed(2)
    }
  }
}
