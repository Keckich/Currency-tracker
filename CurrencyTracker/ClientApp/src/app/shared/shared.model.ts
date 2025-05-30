import { ValidatorFn } from "@angular/forms"
import { TradeSignalType } from "./shared.enum"

export interface Trade {
  id: number,
  position?: number,
  price: number,
  date: Date,
  currency: string,
  amount: number,
  value: number,
  takeProfit?: number,
  stopLoss?: number,
}

export interface TradesPaginationData {
  data: Trade[],
  totalItems: number,
}

export interface PnLData {
  date: Date,
  currency: string,
  balance: number,
  pnL: number,
}

export interface Transaction {
  amount: (number | ValidatorFn[])[],
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

export interface AnalysisResult {
  position?: number,
  currency: string,
  avgPrice: number,
  roi: number,
  recommendation: string,
  tradeInfo: AnalyzedTradeInfo,
}

export interface AnalyzedTradeInfo {
  totalAmount: number,
  totalSpent: number,
}

export interface LimitOrder {
  takeProfit: number[],
  stopLoss: number[],
}

export interface Order {
  price: number;
  amount: number;
}

export interface OrderBook {
  bids: Order[];
  asks: Order[];
}

export interface OrderBookData {
  b: [number, number][];
  a: [number, number][];
}

export interface TradeSignal {
  Type: TradeSignalType,
  Confidence: number,
}

export class RouteParams {
  id?: string | undefined = undefined
}
