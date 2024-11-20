export interface Trade {
  price: number,
  date: Date,
  currency: string,
  amount: number,
}

export interface ChartData {
  x: number,
  y: [number, number, number, number]
}

export interface CandleData {
  open: number,
  high: number,
  low: number,
  close: number,
}

export interface Currency {
  symbol: string,
  status: string,
  basseAsset: string,
  quoteAsset: string,
}

export interface RouteParams {
  id: number,
}

export enum SortOrder {
  Asc = 'asc',
  Desc = 'desc',
}

export enum ChartInterval {
  S1 = '1s',
  M1 = '1m',
  M15 = '15m',
  H1 = '1h',
  H4 = '4h',
  H12 = '12h',
  D1 = '1d',
}
