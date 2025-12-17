export interface VehicleSearchResult {
  id: string;
  make: string;
  model: string;
  year: number;
  price: number;
  mileage: number;
  location: string;
  description?: string;
  imageUrl?: string;
  listingUrl?: string;
  listedDate?: Date;
}

export interface VehicleSearchResultDto {
  results: VehicleSearchResult[];
  totalCount: number;
  searchCriteria: any;
}
