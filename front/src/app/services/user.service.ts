import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map, catchError, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { User, ApiResponse, CreateUserRequest } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly apiBaseUrl = environment.apiBaseUrl;
  private http = inject(HttpClient);

  getUsers(name?: string): Observable<User[]> {
    let params = new HttpParams();
    if (name) {
      params = params.set('name', name);
    }

    return this.http.get<ApiResponse<User[]>>(
      `${this.apiBaseUrl}/api/users`,
      { params }
    ).pipe(
      map(response => response.data || []),
      catchError(error => {
        console.error('Error fetching users:', error);
        return throwError(() => error);
      })
    );
  }

  getUserById(id: string): Observable<User | null> {
    return this.getUsers().pipe(
      map(users => users.find(u => u.id === id) || null)
    );
  }

  createUser(user: CreateUserRequest): Observable<User> {
    return this.http.post<ApiResponse<User>>(
      `${this.apiBaseUrl}/api/users`,
      user
    ).pipe(
      map(response => {
        if (response.data) {
          return response.data;
        }
        throw new Error('Invalid response from server');
      }),
      catchError(error => {
        console.error('Error creating user:', error);
        return throwError(() => error);
      })
    );
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<ApiResponse<void>>(
      `${this.apiBaseUrl}/api/users/${id}`
    ).pipe(
      map(() => undefined),
      catchError(error => {
        console.error('Error deleting user:', error);
        return throwError(() => error);
      })
    );
  }
}
