import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { VehicleSearchCriteria, VehicleSearchResultDto } from '../models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class VehicleSearchService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  searchVehicles(criteria: VehicleSearchCriteria): Observable<VehicleSearchResultDto[]> {
    return this.http.post<VehicleSearchResultDto[]>(`${this.apiUrl}/vehicles/search`, criteria);
  }
}
