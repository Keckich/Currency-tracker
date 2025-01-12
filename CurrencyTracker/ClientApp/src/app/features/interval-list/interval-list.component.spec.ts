import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IntervalListComponent } from './interval-list.component';
import { ChartIntervals } from '../../shared/constants.value';
import { ChartInterval } from '../../shared/shared.enum';
import { By } from '@angular/platform-browser';

describe('IntervalListComponent', () => {
  let component: IntervalListComponent<ChartInterval>;
  let fixture: ComponentFixture<IntervalListComponent<ChartInterval>>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IntervalListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IntervalListComponent<ChartInterval>);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display intervals in list', () => {
    const intervals: { key: ChartInterval; value: string } [] = Object.entries(ChartIntervals).map(([key, value]) => ({
      key: key as ChartInterval,
      value
    }));
    fixture.detectChanges();

    const listElements = fixture.debugElement.queryAll(By.css('li'));
    expect(listElements.length).toBe(intervals.length);

    const firstElement = listElements[0].nativeElement;
    expect(firstElement.textContent).toContain(intervals[0].value);
  });
});
