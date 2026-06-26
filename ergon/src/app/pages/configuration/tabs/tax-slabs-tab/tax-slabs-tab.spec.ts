import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaxSlabsTab } from './tax-slabs-tab';

describe('TaxSlabsTab', () => {
  let component: TaxSlabsTab;
  let fixture: ComponentFixture<TaxSlabsTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaxSlabsTab],
    }).compileComponents();

    fixture = TestBed.createComponent(TaxSlabsTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
