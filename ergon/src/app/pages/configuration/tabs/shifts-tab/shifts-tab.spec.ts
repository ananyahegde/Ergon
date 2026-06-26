import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShiftsTab } from './shifts-tab';

describe('ShiftsTab', () => {
  let component: ShiftsTab;
  let fixture: ComponentFixture<ShiftsTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShiftsTab],
    }).compileComponents();

    fixture = TestBed.createComponent(ShiftsTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
