import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DesignationsTab } from './designations-tab';

describe('DesignationsTab', () => {
  let component: DesignationsTab;
  let fixture: ComponentFixture<DesignationsTab>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DesignationsTab],
    }).compileComponents();

    fixture = TestBed.createComponent(DesignationsTab);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
