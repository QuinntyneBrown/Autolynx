import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { VehicleSearch } from './pages/vehicle-search/vehicle-search';
import { VehicleSearchResults } from './pages/vehicle-search-results/vehicle-search-results';
import { Dashboard } from './pages/dashboard/dashboard';
import { Login } from './pages/login/login';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'login', component: Login },
  { path: 'search', component: VehicleSearch, canActivate: [authGuard] },
  { path: 'results', component: VehicleSearchResults, canActivate: [authGuard] },
  { path: 'dashboard', component: Dashboard, canActivate: [authGuard] },
  { path: '**', redirectTo: '' }
];
