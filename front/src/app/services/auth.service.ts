import { Injectable, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, catchError, throwError, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest, LoginResponse, ApiResponse } from '../models/user.model';

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
        if (response.data) {
          this.setAuthData(response.data.accessToken);
          this._isAuthenticated.set(true);
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

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
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
}

