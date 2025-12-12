export interface User {
  id?: string;
  name: string;
  email: string;
  createdAt?: string;
  active?: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  tokenType: string;
  expiresIn: number;
}

export interface ApiResponse<T> {
  status: string;
  message: string;
  data: T;
}

export interface CreateUserRequest {
  name: string;
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

