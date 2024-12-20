import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IntervalListComponent } from './interval-list.component';

describe('IntervalListComponent', () => {
  let component: IntervalListComponent;
  let fixture: ComponentFixture<IntervalListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IntervalListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IntervalListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
