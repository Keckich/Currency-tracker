import { Trade } from "./shared.model";

export const Constants = {
  CURRENCIES: ['BTCUSDT', 'ETHUSDT', 'BNBUSDT'],
  DISPLAYED_COLUMNS: ['position', 'price', 'date', 'currency', 'amount'] as (keyof Trade)[],
  CANDLE_COLORS: { green: '#00B746', red: '#EF403C' },
  CANDLE_CHART_SIZE: 100,
  CHART_TIME_INTERVALS: ['1s', '1m', '15m', '1h', '4h', '12h', '1d']
} as const
