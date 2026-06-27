import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { AuthService } from '../../core/auth/auth.service';
import { ProfileService } from '../../core/services/profile.service';
import { MasterService } from '../../core/services/master.service';
import { ToastService } from '../../core/services/toast.service';
import { EmployeeDetailResponse } from '../../core/models/employee.model';
import { SingleSelectDropdown } from '../../shared/components/single-select-dropdown/single-select-dropdown';
import { DatePipe } from '@angular/common';

function passwordStrengthValidator(control: AbstractControl): ValidationErrors | null {
  const value = control.value;
  if (!value) return null;
  const valid = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/.test(value);
  return valid ? null : { passwordStrength: true };
}

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [ReactiveFormsModule, SingleSelectDropdown, DatePipe],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class Profile implements OnInit {
  private authService = inject(AuthService);
  private profileService = inject(ProfileService);
  private masterService = inject(MasterService);
  private toastService = inject(ToastService);

  currentUser = this.authService.currentUser;
  countries = this.masterService.countries;
  states = this.masterService.states;
  cities = this.masterService.cities;

  employee = signal<EmployeeDetailResponse | null>(null);
  isLoading = signal(false);
  avatarUrl = signal<string | null>(null);
  uploadingPfp = signal(false);

  activeTab = signal<'personal' | 'employment' | 'security'>('personal');

  profileForm = new FormGroup({
    firstName: new FormControl('', [Validators.required, Validators.minLength(2)]),
    lastName: new FormControl('', [Validators.required, Validators.minLength(2)]),
    personalEmail: new FormControl('', [Validators.required, Validators.email]),
    phone: new FormControl('', [Validators.required]),
    addressLine1: new FormControl('', [Validators.required]),
    addressLine2: new FormControl(''),
    countryId: new FormControl<number | null>(null, Validators.required),
    stateId: new FormControl<number | null>(null, Validators.required),
    cityId: new FormControl<number | null>(null, Validators.required),
  });

  passwordForm = new FormGroup({
    oldPassword: new FormControl('', [Validators.required]),
    newPassword: new FormControl('', [Validators.required, Validators.minLength(8), passwordStrengthValidator]),
    confirmPassword: new FormControl('', [Validators.required]),
  }, { validators: this.passwordMatchValidator });

  savingProfile = signal(false);
  savingPassword = signal(false);
  showOldPassword = signal(false);
  showNewPassword = signal(false);
  showConfirmPassword = signal(false);

  ngOnInit() {
    const id = this.currentUser()?.id;
    if (!id) return;

    this.isLoading.set(true);
    this.profileService.getProfile(id).subscribe({
      next: (emp) => {
        this.employee.set(emp);
        this.patchProfileForm(emp);
        this.isLoading.set(false);
        this.loadPfp(id);
      },
      error: () => this.isLoading.set(false)
    });
  }

  private patchProfileForm(emp: EmployeeDetailResponse) {
    const country = this.countries().find(c => c.countryName === emp.countryName);
    const state = this.states().find(s => s.stateName === emp.stateName);
    const city = this.cities().find(c => c.cityName === emp.cityName);

    this.profileForm.patchValue({
      firstName: emp.firstName,
      lastName: emp.lastName,
      personalEmail: emp.personalEmail,
      phone: emp.phone,
      addressLine1: emp.addressLine1,
      addressLine2: emp.addressLine2,
      countryId: country?.countryId ?? null,
      stateId: state?.stateId ?? null,
      cityId: city?.cityId ?? null,
    });
  }

  private loadPfp(id: string) {
    this.profileService.getPfp(id).subscribe({
      next: (blob) => {
        this.avatarUrl.set(URL.createObjectURL(blob));
      },
      error: () => this.avatarUrl.set(null)
    });
  }

  getInitials() {
    const emp = this.employee();
    if (!emp) return '';
    return `${emp.firstName.charAt(0)}${emp.lastName.charAt(0)}`.toUpperCase();
  }

  onPfpChange(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;

    const id = this.currentUser()?.id;
    if (!id) return;

    this.uploadingPfp.set(true);
    this.profileService.updatePfp(id, file).subscribe({
      next: () => {
        this.loadPfp(id);
        this.toastService.success('Profile picture updated.');
        this.uploadingPfp.set(false);
      },
      error: (err) => {
        this.toastService.error(err?.error?.message ?? 'Failed to upload photo.');
        this.uploadingPfp.set(false);
      }
    });
  }

  saveProfile() {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      return;
    }

    const id = this.currentUser()?.id;
    if (!id) return;

    const v = this.profileForm.value;

    const request = {
      firstName: v.firstName!,
      lastName: v.lastName!,
      personalEmail: v.personalEmail!,
      phone: v.phone!,
      addressLine1: v.addressLine1!,
      addressLine2: v.addressLine2 ?? undefined,
      cityId: v.cityId!,
      stateId: v.stateId!,
      countryId: v.countryId!,
    };

    this.savingProfile.set(true);
    this.profileService.updateProfile(id, request as any).subscribe({
      next: (updated) => {
        this.employee.set(updated);
        this.toastService.success('Profile updated.');
        this.savingProfile.set(false);
      },
      error: (err) => {
        this.toastService.error(err?.error?.message ?? 'Something went wrong.');
        this.savingProfile.set(false);
      }
    });
  }

  changePassword() {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }

    const v = this.passwordForm.value;
    this.savingPassword.set(true);
    this.profileService.changePassword({ oldPassword: v.oldPassword!, newPassword: v.newPassword! }).subscribe({
      next: () => {
        this.toastService.success('Password changed successfully.');
        this.passwordForm.reset();
        this.savingPassword.set(false);
      },
      error: (err) => {
        this.toastService.error(err?.error?.message ?? 'Something went wrong.');
        this.savingPassword.set(false);
      }
    });
  }

  private passwordMatchValidator(group: AbstractControl): ValidationErrors | null {
    const newPassword = group.get('newPassword')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return newPassword === confirmPassword ? null : { passwordMismatch: true };
  }

  hasError(form: FormGroup, field: string, error: string) {
    const control = form.get(field);
    return control?.hasError(error) && control?.touched;
  }

  formGroupHasError(error: string) {
    return this.passwordForm.hasError(error) && this.passwordForm.get('confirmPassword')?.touched;
  }
}