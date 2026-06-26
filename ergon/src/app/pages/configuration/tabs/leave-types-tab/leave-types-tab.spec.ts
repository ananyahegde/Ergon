import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LeaveTypesTab } from './leave-types-tab';

describe('LeaveTypesTab', () => {
  let component: LeaveTypesTab;
  let fixture: ComponentFixture<LeaveTypesTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LeaveTypesTab],
    }).compileComponents();

    fixture = TestBed.createComponent(LeaveTypesTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
