// Copyright (c) Quinntyne Brown. All Rights Reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace Autolynx.Core.Models;

public class VehicleSearchResultDto
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int Year { get; set; }
    public string? Trim { get; set; }
    public int Mileage { get; set; }
    public string? Color { get; set; }
    public string? Transmission { get; set; }
    public string? FuelType { get; set; }
    public decimal Price { get; set; }
    public bool IsGoodPrice { get; set; }
    public string? ListingUrl { get; set; }
    public string? DealerName { get; set; }
    public string? SellerPhone { get; set; }
    public string? SellerEmail { get; set; }
    public string? Location { get; set; }
    public string? Source { get; set; }
    public string? VIN { get; set; }
}
