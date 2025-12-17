import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { VehicleSearchCriteria, VehicleSearchResultDto } from '../models';

@Injectable({
  providedIn: 'root'
})
export class VehicleSearchService {
  private apiUrl = 'http://localhost:5000/api'; // TODO: Move to environment config

  constructor(private http: HttpClient) {}

  searchVehicles(criteria: VehicleSearchCriteria): Observable<VehicleSearchResultDto> {
    let params = new HttpParams();
    
    if (criteria.make) params = params.set('make', criteria.make);
    if (criteria.model) params = params.set('model', criteria.model);
    if (criteria.yearFrom) params = params.set('yearFrom', criteria.yearFrom.toString());
    if (criteria.yearTo) params = params.set('yearTo', criteria.yearTo.toString());
    if (criteria.priceFrom) params = params.set('priceFrom', criteria.priceFrom.toString());
    if (criteria.priceTo) params = params.set('priceTo', criteria.priceTo.toString());
    if (criteria.mileageFrom) params = params.set('mileageFrom', criteria.mileageFrom.toString());
    if (criteria.mileageTo) params = params.set('mileageTo', criteria.mileageTo.toString());
    if (criteria.location) params = params.set('location', criteria.location);
    if (criteria.radius) params = params.set('radius', criteria.radius.toString());

    return this.http.get<VehicleSearchResultDto>(`${this.apiUrl}/vehicles/search`, { params });
  }
}
