import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TradesComponent } from './trades.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

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
});
