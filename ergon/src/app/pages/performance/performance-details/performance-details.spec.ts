import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PerformanceDetails } from './performance-details';

describe('PerformanceDetails', () => {
  let component: PerformanceDetails;
  let fixture: ComponentFixture<PerformanceDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PerformanceDetails],
    }).compileComponents();

    fixture = TestBed.createComponent(PerformanceDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
