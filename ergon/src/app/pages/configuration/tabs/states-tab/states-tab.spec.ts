import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StatesTab } from './states-tab';

describe('StatesTab', () => {
  let component: StatesTab;
  let fixture: ComponentFixture<StatesTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatesTab],
    }).compileComponents();

    fixture = TestBed.createComponent(StatesTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
