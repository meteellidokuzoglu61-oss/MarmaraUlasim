import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: 'home',
    loadComponent: () =>
      import('./home/home.page').then((m) => m.HomePage),
  },
  {
    path: 'hatlar',
    loadComponent: () =>
      import('./pages/hatlar/hatlar.page').then((m) => m.HatlarPage),
  },
  {
    path: 'duraklar',
    loadComponent: () =>
      import('./pages/duraklar/duraklar.page').then((m) => m.DuraklarPage),
  },
  {
    path: 'harita',
    loadComponent: () =>
      import('./pages/harita/harita.page').then((m) => m.HaritaPage),
  },
  {
    path: 'favoriler',
    loadComponent: () =>
      import('./pages/favoriler/favoriler.page').then((m) => m.FavorilerPage),
  },
  {
    path: 'iller',
    loadComponent: () => import('./pages/iller/iller.page').then( m => m.IllerPage)
  },
  {
    path: 'il-detay',
    loadComponent: () => import('./pages/il-detay/il-detay.page').then( m => m.IlDetayPage)
  },
  {
    path: 'hatlar',
    loadComponent: () => import('./pages/hatlar/hatlar.page').then( m => m.HatlarPage)
  },
  {
    path: 'duraklar',
    loadComponent: () => import('./pages/duraklar/duraklar.page').then( m => m.DuraklarPage)
  },
  {
    path: 'harita',
    loadComponent: () => import('./pages/harita/harita.page').then( m => m.HaritaPage)
  },
  {
    path: 'favoriler',
    loadComponent: () => import('./pages/favoriler/favoriler.page').then( m => m.FavorilerPage)
  },
];