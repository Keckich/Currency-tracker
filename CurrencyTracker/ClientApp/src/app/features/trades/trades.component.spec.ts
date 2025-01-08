import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TradesComponent } from './trades.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { By } from '@angular/platform-browser';
import { MatTableDataSource } from '@angular/material/table';

describe('TradesComponent', () => {
  let component: TradesComponent;
  let fixture: ComponentFixture<TradesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TradesComponent, BrowserAnimationsModule]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TradesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display trades in table', () => {
    const trades = [
      { id: 1, price: 90000, date: new Date(), currency: 'BTC', amount: 1.2, value: 90000 * 1.2 },
      { id: 2, price: 3500, date: new Date(), currency: 'ETH', amount: 0.2, value: 3500 * 0.2 }
    ]
    component.dataSource = new MatTableDataSource(trades);
    fixture.detectChanges();

    const rows = fixture.debugElement.queryAll(By.css('.mat-mdc-row'));
    expect(rows.length).toBe(trades.length);

    const firstRow = rows[0].nativeElement;
    expect(firstRow.textContent).toContain(trades[0].price);
    expect(firstRow.textContent).toContain(trades[0].currency);
  })
});
