export interface User {
  id: number;
  name: string;
  role: string;
  location: string;
  currentDeviceName?: string;
}

export interface UserRequest {
  name: string;
  role: string;
  location: string;
}

export interface AdminCreateUserRequest {
  email: string;
  password: string;
  name: string;
  location: string;
  role: string;
}
