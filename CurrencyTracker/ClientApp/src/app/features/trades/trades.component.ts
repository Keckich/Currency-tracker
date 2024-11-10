import { Component, Input } from '@angular/core';
import { Trade } from '../../shared/models/trade';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-trades',
  standalone: true,
  imports: [
    MatTableModule
  ],
  templateUrl: './trades.component.html',
  styleUrl: './trades.component.css'
})
export class TradesComponent {
  @Input() trades: Trade[] = [];
  displayedColumns: (keyof Trade)[] = ['price', 'date', 'currency', 'number'];
}
