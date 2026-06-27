import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { DepartmentsTab } from './tabs/departments-tab/departments-tab';
import { BranchesTab } from './tabs/branches-tab/branches-tab';
import { DesignationsTab } from './tabs/designations-tab/designations-tab';
import { RolesTab } from './tabs/roles-tab/roles-tab';
import { CountriesTab } from './tabs/countries-tab/countries-tab';
import { StatesTab } from './tabs/states-tab/states-tab';
import { CitiesTab } from './tabs/cities-tab/cities-tab';
import { ShiftsTab } from './tabs/shifts-tab/shifts-tab';
import { LeaveTypesTab } from './tabs/leave-types-tab/leave-types-tab';
import { PublicHolidaysTab } from './tabs/public-holidays-tab/public-holidays-tab';
import { TaxSlabsTab } from './tabs/tax-slabs-tab/tax-slabs-tab';
import { LeaveEntitlementsTab } from './tabs/leave-entitlements-tab/leave-entitlements-tab';
import { SalaryStructuresTab } from './tabs/salary-structures-tab/salary-structures-tab';

const TABS = [
  { key: 'departments', label: 'Departments' },
  { key: 'branches', label: 'Branches' },
  { key: 'designations', label: 'Designations' },
  { key: 'roles', label: 'Roles' },
  { key: 'shifts', label: 'Shifts' },
  { key: 'leave-types', label: 'Leave Types' },
  { key: 'leave-entitlements', label: 'Leave Entitlements' },
  { key: 'salary-structures', label: 'Salary Structures' },
  { key: 'public-holidays', label: 'Public Holidays' },
  { key: 'tax-slabs', label: 'Tax Slabs' },
  { key: 'countries', label: 'Countries' },
  { key: 'states', label: 'States' },
  { key: 'cities', label: 'Cities' },
] as const;

type TabKey = typeof TABS[number]['key'];

@Component({
  selector: 'app-configuration',
  standalone: true,
  imports: [
    DepartmentsTab, 
    BranchesTab, 
    DesignationsTab, 
    RolesTab, 
    CountriesTab, 
    StatesTab, 
    CitiesTab, 
    ShiftsTab, 
    LeaveTypesTab, 
    PublicHolidaysTab, 
    TaxSlabsTab,
    LeaveEntitlementsTab,
    SalaryStructuresTab
  ],
  templateUrl: './configuration.html',
  styleUrl: './configuration.css'
})
export class Configuration implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  readonly tabs = TABS;
  activeTab = signal<TabKey>('departments');

  ngOnInit() {
    this.route.queryParamMap.subscribe(params => {
      const tab = params.get('tab') as TabKey;
      if (tab && this.tabs.some(t => t.key === tab)) {
        this.activeTab.set(tab);
      } else {
        this.navigateTo('departments');
      }
    });
  }

  navigateTo(tab: TabKey) {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { tab },
      queryParamsHandling: 'merge'
    });
  }
}