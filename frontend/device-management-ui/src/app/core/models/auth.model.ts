export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  name: string;
  location: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  role: string;
  userId?: number;
}

export interface SelfAssignRequest {
  deviceId: number;
  location: string;
}
