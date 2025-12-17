export interface VehicleSearchCriteria {
  make?: string;
  model?: string;
  yearFrom?: number;
  yearTo?: number;
  priceFrom?: number;
  priceTo?: number;
  mileageFrom?: number;
  mileageTo?: number;
  location?: string;
  radius?: number;
}
