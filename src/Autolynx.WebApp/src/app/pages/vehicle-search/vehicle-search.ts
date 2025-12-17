import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { VehicleSearchCriteria } from '../../models';

@Component({
  selector: 'app-vehicle-search',
  imports: [FormsModule, MatInputModule, MatButtonModule, MatCardModule, MatFormFieldModule],
  templateUrl: './vehicle-search.html',
  styleUrl: './vehicle-search.scss',
})
export class VehicleSearch {
  searchCriteria: VehicleSearchCriteria = {};

  constructor(private router: Router) {}

  onSearch(): void {
    const queryParams: any = {};
    
    if (this.searchCriteria.make) queryParams.make = this.searchCriteria.make;
    if (this.searchCriteria.model) queryParams.model = this.searchCriteria.model;
    if (this.searchCriteria.yearFrom) queryParams.yearFrom = this.searchCriteria.yearFrom;
    if (this.searchCriteria.yearTo) queryParams.yearTo = this.searchCriteria.yearTo;
    if (this.searchCriteria.priceFrom) queryParams.priceFrom = this.searchCriteria.priceFrom;
    if (this.searchCriteria.priceTo) queryParams.priceTo = this.searchCriteria.priceTo;
    if (this.searchCriteria.mileageFrom) queryParams.mileageFrom = this.searchCriteria.mileageFrom;
    if (this.searchCriteria.mileageTo) queryParams.mileageTo = this.searchCriteria.mileageTo;
    if (this.searchCriteria.location) queryParams.location = this.searchCriteria.location;
    if (this.searchCriteria.radius) queryParams.radius = this.searchCriteria.radius;

    this.router.navigate(['/results'], { queryParams });
  }

  onReset(): void {
    this.searchCriteria = {};
  }
}
