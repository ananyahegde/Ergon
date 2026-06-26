import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RolesTab } from './roles-tab';

describe('RolesTab', () => {
  let component: RolesTab;
  let fixture: ComponentFixture<RolesTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RolesTab],
    }).compileComponents();

    fixture = TestBed.createComponent(RolesTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
