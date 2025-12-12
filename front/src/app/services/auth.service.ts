import { Injectable, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, throwError, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest, LoginResponse, ApiResponse, RegisterRequest, User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly apiBaseUrl = environment.apiBaseUrl;

  private http = inject(HttpClient);
  private router = inject(Router);

  private _isAuthenticated = signal<boolean>(this.isTokenValid());
  isAuthenticated = this._isAuthenticated.asReadonly();

  private _currentUser = signal<User | null>(this.getUserFromToken());
  currentUser = this._currentUser.asReadonly();

  constructor() {
    if (this.isTokenValid()) {
      this._isAuthenticated.set(true);
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<ApiResponse<LoginResponse>>(
      `${this.apiBaseUrl}/api/auth/login`,
      credentials
    ).pipe(
      map(response => {
        if (response.data && response.data.accessToken) {
          const token = response.data.accessToken.trim();
          this.setAuthData(token);
          this._isAuthenticated.set(true);
          this._currentUser.set(this.getUserFromToken());
          return response.data;
        }
        throw new Error('Invalid response from server');
      }),
      catchError(error => {
        console.error('Login error:', error);
        return throwError(() => error);
      })
    );
  }

  register(data: RegisterRequest): Observable<void> {
    return this.http.post<ApiResponse<object>>(
      `${this.apiBaseUrl}/api/auth/register`,
      data
    ).pipe(
      map(() => undefined),
      catchError(error => {
        console.error('Register error:', error);
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this._currentUser.set(null);
    this._isAuthenticated.set(false);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  checkAuthentication(): boolean {
    return this.isTokenValid();
  }

  private isTokenValid(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expirationTime = payload.exp * 1000;
      return Date.now() < expirationTime;
    } catch {
      return false;
    }
  }

  private setAuthData(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  private getUserFromToken(): User | null {
    const token = this.getToken();
    if (!token) return null;
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return {
        id: payload.sub || null,
        name: payload.given_name || payload.name || '',
        email: payload.email || ''
      };
    } catch {
      return null;
    }
  }
}
