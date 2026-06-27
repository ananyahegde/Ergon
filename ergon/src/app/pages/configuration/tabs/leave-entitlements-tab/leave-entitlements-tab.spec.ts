import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LeaveEntitlementsTab } from './leave-entitlements-tab';

describe('LeaveEntitlementsTab', () => {
  let component: LeaveEntitlementsTab;
  let fixture: ComponentFixture<LeaveEntitlementsTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LeaveEntitlementsTab],
    }).compileComponents();

    fixture = TestBed.createComponent(LeaveEntitlementsTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
