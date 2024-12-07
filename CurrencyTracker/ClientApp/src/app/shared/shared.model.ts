export interface Trade {
  price: number,
  date: Date,
  currency: string,
  amount: number,
  value: number,
  takeProfit: number | undefined,
  stopLoss: number | undefined,
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

export class RouteParams {
  id?: string | undefined = undefined
}
