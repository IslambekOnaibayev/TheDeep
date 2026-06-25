import { Routes } from '@angular/router';
import { requireSession } from './core/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/sign-in/sign-in').then((m) => m.SignIn),
  },
  {
    path: 'lobby',
    canMatch: [requireSession],
    loadComponent: () => import('./features/lobby/lobby').then((m) => m.Lobby),
  },
  {
    path: 'game/:id',
    canMatch: [requireSession],
    loadComponent: () => import('./features/game/game').then((m) => m.Game),
  },
  {
    path: 'leaderboard',
    loadComponent: () => import('./features/leaderboard/leaderboard').then((m) => m.Leaderboard),
  },
  {
    path: 'profile',
    canMatch: [requireSession],
    loadComponent: () => import('./features/profile/profile').then((m) => m.Profile),
  },
  {
    path: 'settings',
    loadComponent: () => import('./features/settings/settings').then((m) => m.Settings),
  },
  { path: '**', redirectTo: '' },
];
