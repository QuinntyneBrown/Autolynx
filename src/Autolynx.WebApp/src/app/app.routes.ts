import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { VehicleSearch } from './pages/vehicle-search/vehicle-search';
import { VehicleSearchResults } from './pages/vehicle-search-results/vehicle-search-results';
import { Dashboard } from './pages/dashboard/dashboard';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'search', component: VehicleSearch },
  { path: 'results', component: VehicleSearchResults },
  { path: 'dashboard', component: Dashboard },
  { path: '**', redirectTo: '' }
];
