import { Component, Input } from '@angular/core';
import { Trade } from '../../shared/shared.model';
import { MatTableModule } from '@angular/material/table';
import { Constants } from '../../shared/constants.value';

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
  displayedColumns = Constants.DISPLAYED_COLUMNS;
}
