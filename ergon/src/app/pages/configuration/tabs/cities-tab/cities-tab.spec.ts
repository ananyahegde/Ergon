import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CitiesTab } from './cities-tab';

describe('CitiesTab', () => {
  let component: CitiesTab;
  let fixture: ComponentFixture<CitiesTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CitiesTab],
    }).compileComponents();

    fixture = TestBed.createComponent(CitiesTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
