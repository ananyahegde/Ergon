export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  personalEmail: string;
  phone: string;
  addressLine1: string;
  addressLine2?: string;
  cityId?: number;
  stateId: number;
  countryId: number;
}