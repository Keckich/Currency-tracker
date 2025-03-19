﻿using CurrencyTracker.Business.Models;

namespace CurrencyTracker.Business.Services.Interfaces
{
    public interface IBinanceWebSocketService
    {
        Task ConnectToStreamAsync(BinanceSocketRequest data);

        Task DisconnectFromStreamAsync(BinanceSocketRequest data);
    }
}
