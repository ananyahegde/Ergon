import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryStructuresTab } from './salary-structures-tab';

describe('SalaryStructuresTab', () => {
  let component: SalaryStructuresTab;
  let fixture: ComponentFixture<SalaryStructuresTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SalaryStructuresTab],
    }).compileComponents();

    fixture = TestBed.createComponent(SalaryStructuresTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
