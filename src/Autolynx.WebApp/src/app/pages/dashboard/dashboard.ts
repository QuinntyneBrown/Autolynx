import { Component, OnInit, OnDestroy, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SignalRService } from '../../services';
import { VehicleSearchResult } from '../../models';

@Component({
  selector: 'app-dashboard',
  imports: [CommonModule, MatCardModule, MatChipsModule, MatProgressSpinnerModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit, OnDestroy {
  latestResults: VehicleSearchResult[] = [];

  constructor(private signalRService: SignalRService) {
    effect(() => {
      const data = this.signalRService.vehicleSearchResults();
      if (data?.results) {
        this.latestResults = data.results;
      }
    });
  }

  get connectionState() {
    return this.signalRService.connectionState;
  }

  ngOnInit(): void {
    this.signalRService.startConnection();
  }

  ngOnDestroy(): void {
    this.signalRService.stopConnection();
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
