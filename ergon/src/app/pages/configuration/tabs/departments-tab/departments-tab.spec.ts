import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DepartmentsTab } from './departments-tab';

describe('DepartmentsTab', () => {
  let component: DepartmentsTab;
  let fixture: ComponentFixture<DepartmentsTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DepartmentsTab],
    }).compileComponents();

    fixture = TestBed.createComponent(DepartmentsTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
