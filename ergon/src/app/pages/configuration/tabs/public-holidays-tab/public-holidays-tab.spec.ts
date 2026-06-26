import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PublicHolidaysTab } from './public-holidays-tab';

describe('PublicHolidaysTab', () => {
  let component: PublicHolidaysTab;
  let fixture: ComponentFixture<PublicHolidaysTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PublicHolidaysTab],
    }).compileComponents();

    fixture = TestBed.createComponent(PublicHolidaysTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
