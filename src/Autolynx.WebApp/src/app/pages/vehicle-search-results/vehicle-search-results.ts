import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { VehicleSearchService } from '../../services';
import { VehicleSearchCriteria, VehicleSearchResult } from '../../models';

@Component({
  selector: 'app-vehicle-search-results',
  imports: [CommonModule, MatCardModule, MatProgressSpinnerModule, MatChipsModule],
  templateUrl: './vehicle-search-results.html',
  styleUrl: './vehicle-search-results.scss',
})
export class VehicleSearchResults implements OnInit {
  results = signal<VehicleSearchResult[]>([]);
  loading = signal<boolean>(false);
  searchCriteria = signal<VehicleSearchCriteria>({});
  error = signal<string | null>(null);

  constructor(
    private route: ActivatedRoute,
    private vehicleSearchService: VehicleSearchService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const criteria: VehicleSearchCriteria = {
        make: params['make'],
        model: params['model'],
        yearFrom: params['yearFrom'] ? +params['yearFrom'] : undefined,
        yearTo: params['yearTo'] ? +params['yearTo'] : undefined,
        priceFrom: params['priceFrom'] ? +params['priceFrom'] : undefined,
        priceTo: params['priceTo'] ? +params['priceTo'] : undefined,
        mileageFrom: params['mileageFrom'] ? +params['mileageFrom'] : undefined,
        mileageTo: params['mileageTo'] ? +params['mileageTo'] : undefined,
        location: params['location'],
        radius: params['radius'] ? +params['radius'] : undefined,
      };

      this.searchCriteria.set(criteria);
      this.performSearch(criteria);
    });
  }

  private performSearch(criteria: VehicleSearchCriteria): void {
    this.loading.set(true);
    this.error.set(null);

    this.vehicleSearchService.searchVehicles(criteria).subscribe({
      next: (response) => {
        this.results.set(response.results || []);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error searching vehicles:', err);
        this.error.set('Failed to load search results. Please try again.');
        this.loading.set(false);
        // For demo purposes, set empty results
        this.results.set([]);
      }
    });
  }

  formatPrice(price: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
      minimumFractionDigits: 0
    }).format(price);
  }

  formatMileage(mileage: number): string {
    return new Intl.NumberFormat('en-US').format(mileage) + ' miles';
  }
}
