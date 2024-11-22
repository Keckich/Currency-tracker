import { ChartInterval, Trade } from "./shared.model";
import { ApexChart, ApexPlotOptions, ApexTitleSubtitle, ApexXAxis, ApexYAxis } from 'ng-apexcharts';

export const Constants = {
  CURRENCIES: ['BTCUSDT', 'ETHUSDT', 'BNBUSDT'],
  DISPLAYED_COLUMNS: ['position', 'price', 'date', 'currency', 'amount'] as (keyof Trade)[],
  CANDLE_COLORS: { green: '#00B746', red: '#EF403C' },
  CANDLE_CHART_SIZE: 100,
} as const

export const Routes = {
  CURRENCY: 'currency',
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
