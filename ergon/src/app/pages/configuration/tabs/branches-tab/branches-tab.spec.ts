import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BranchesTab } from './branches-tab';

describe('BranchesTab', () => {
  let component: BranchesTab;
  let fixture: ComponentFixture<BranchesTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BranchesTab],
    }).compileComponents();

    fixture = TestBed.createComponent(BranchesTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
