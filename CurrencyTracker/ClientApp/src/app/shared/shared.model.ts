export interface Trade {
  price: number,
  date: Date,
  currency: string,
  amount: number,
}

export interface ChartData {
  x: number,
  y: number,
}


export interface Currency {
  symbol: string,
  status: string,
  basseAsset: string,
  quoteAsset: string,
}
