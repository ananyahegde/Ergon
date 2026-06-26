import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CountriesTab } from './countries-tab';

describe('CountriesTab', () => {
  let component: CountriesTab;
  let fixture: ComponentFixture<CountriesTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CountriesTab],
    }).compileComponents();

    fixture = TestBed.createComponent(CountriesTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
