import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TagsGeneratorService {
  generateTooltip(date: number, balance: number, pnl: number): string {
    const formattedDate = new Date(date).toLocaleString();
    return `
        <div class="p-2.5 rounded border border-solid border-gray-400 bg-white text-black">
          <strong>${formattedDate}</strong><br>
          Balance: $${balance.toFixed(2)}<br>
          PnL: ${pnl.toFixed(2)}%
        </div>
      `;
  }
}
