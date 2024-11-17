export interface Trade {
  price: number,
  date: Date,
  currency: string,
  amount: number,
}

export interface CandleData {
  startTime: number,
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
