import { Routes } from '@angular/router';
import { authGuard } from './app.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/home/home.page').then((m) => m.HomePage),
    title: 'Parent2Parent'
  },
  {
    path: 'register',
    loadComponent: () => import('./pages/register/register.page').then((m) => m.RegisterPage),
    title: 'Register • Parent2Parent'
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.page').then((m) => m.LoginPage),
    title: 'Login • Parent2Parent'
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard.page').then((m) => m.DashboardPage),
    title: 'Dashboard • Parent2Parent',
    canActivate: [authGuard]
  },
  {
    path: 'requests',
    loadComponent: () => import('./pages/requests/requests.page').then((m) => m.RequestsPage),
    title: 'Requests • Parent2Parent',
    canActivate: [authGuard]
  },
  {
    path: 'chat',
    loadComponent: () => import('./pages/chat/chat.page').then((m) => m.ChatPage),
    title: 'Chat • Parent2Parent',
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '' }
];
