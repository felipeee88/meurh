import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./login/login').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./register/register').then(m => m.RegisterComponent)
  },
  {
    path: 'home',
    loadComponent: () => import('./home/home').then(m => m.HomeComponent),
    canActivate: [authGuard]
  },
  {
    path: 'users',
    loadComponent: () => import('./user-list/user-list').then(m => m.UserListComponent),
    canActivate: [authGuard]
  },
  {
    path: 'users/new',
    loadComponent: () => import('./user-form/user-form').then(m => m.UserFormComponent),
    canActivate: [authGuard]
  },
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: '/login'
  }
];
