import { Trade } from "./shared.model";

export const Constants = {
  CURRENCIES: ['BTCUSDT', 'ETHUSDT', 'BNBUSDT'],
  DISPLAYED_COLUMNS: ['price', 'date', 'currency', 'number'] as (keyof Trade)[],
} as const
