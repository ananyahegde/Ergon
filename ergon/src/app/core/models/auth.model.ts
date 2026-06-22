export interface LoginRequest {
  workEmail: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  role: string;
}

export interface CurrentUser {
  id: string;
  email: string;
  firstName: string;
  role: string;
}
