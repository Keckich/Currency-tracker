using CurrencyTracker.Business.Models;
using CurrencyTracker.Business.Models.Backtest;
using CurrencyTracker.Business.Models.Enums;
using CurrencyTracker.Business.Services.Interfaces;

namespace CurrencyTracker.Business.Services
{
    public class BacktestService : IBacktestService
    {
        private readonly IPredictionService predictionService;

        public BacktestService(IPredictionService predictionService)
        {
            this.predictionService = predictionService;
        }

        public BacktestResult Run(IList<Candlestick> candles, decimal initialBalance = 1000m)
        {
            decimal balance = initialBalance;
            decimal coins = 0;
            decimal maxBalance = initialBalance;
            int wins = 0, losses = 0, trades = 0;
            bool inPosition = false;
            decimal entryPrice = 0;

            for (int i = 50; i < candles.Count; i++)
            {
                var window = candles.Take(i + 1);
                var signal = predictionService.GenerateSignal(window);
                var price = (decimal)candles[i].Close;

                if (signal == TradeSignalType.Buy && !inPosition)
                {
                    coins = balance / price;
                    entryPrice = price;
                    balance = 0;
                    inPosition = true;
                    trades++;
                }
                else if (signal == TradeSignalType.Sell && inPosition)
                {
                    balance = coins * price;
                    inPosition = false;

                    if (price > entryPrice) wins++;
                    else losses++;

                    coins = 0;
                }

                maxBalance = Math.Max(maxBalance, balance + coins * price);
            }

            if (inPosition)
            {
                balance = coins * (decimal)candles[^1].Close;
            }

            return new BacktestResult
            {
                StartBalance = initialBalance,
                FinalBalance = balance,
                TotalTrades = trades,
                WinningTrades = wins,
                LosingTrades = losses,
                MaxDrawdown = maxBalance == 0 ? 0 : (maxBalance - balance) / maxBalance * 100
            };
        }
    }
}
