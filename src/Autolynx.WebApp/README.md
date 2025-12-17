# Autolynx Web Application

An Angular Single Page Application for searching vehicles with real-time updates.

## Features

- **Home Page**: Landing page with navigation to main features
- **Vehicle Search**: Advanced search form with multiple criteria (make, model, year, price, mileage, location)
- **Search Results**: Display results with deep linking support - search criteria is preserved in URL query parameters
- **Dashboard**: Real-time vehicle search updates via SignalR integration
- **Material Design**: Clean, modern UI using Angular Material
- **Responsive Layout**: Mobile-friendly design with consistent spacing using design tokens
- **BEM Naming Convention**: All styles follow Block Element Modifier methodology

## Technology Stack

- **Angular 21**: Latest version of Angular framework
- **Angular Material**: Material Design component library
- **TypeScript**: Type-safe development
- **SCSS**: Styling with design tokens
- **SignalR**: Real-time communication with backend
- **Playwright**: End-to-end testing framework

## Project Structure

```
src/
├── app/
│   ├── components/     # Reusable components (barrel exported)
│   ├── config/         # Design tokens and configuration
│   ├── models/         # TypeScript interfaces and DTOs (barrel exported)
│   ├── pages/          # Page components (barrel exported)
│   │   ├── home/
│   │   ├── vehicle-search/
│   │   ├── vehicle-search-results/
│   │   └── dashboard/
│   ├── services/       # API and SignalR services (barrel exported)
│   ├── app.config.ts   # Application configuration
│   ├── app.routes.ts   # Route definitions
│   └── app.ts          # Root component
├── index.html
├── main.ts
└── styles.scss
e2e/                    # Playwright end-to-end tests
├── home.spec.ts
├── vehicle-search.spec.ts
├── vehicle-search-results.spec.ts
└── dashboard.spec.ts
```

## Getting Started

### Prerequisites

- Node.js (v20 or higher)
- npm (v10 or higher)

### Installation

```bash
cd src/Autolynx.WebApp
npm install
```

### Development Server

```bash
npm start
```

Navigate to `http://localhost:4200/`. The application will automatically reload when you change any source files.

### Build

```bash
npm run build
```

The build artifacts will be stored in the `dist/` directory.

### Running Tests

#### End-to-End Tests

```bash
# Run tests in headless mode
npm run e2e

# Run tests with UI
npm run e2e:ui

# Run tests in headed browser mode
npm run e2e:headed
```

## Deep Linking

The search results page supports deep linking. You can share URLs with search criteria:

Example:
```
http://localhost:4200/results?make=Toyota&model=Camry&yearFrom=2020&yearTo=2024&priceFrom=15000&priceTo=30000
```

The application will parse the query parameters and display the search criteria and fetch results automatically.

## Backend Requirements

The application expects the following backend endpoints:

- **API Base URL**: `http://localhost:5000/api`
- **Search Endpoint**: `GET /api/vehicles/search`
- **SignalR Hub**: `http://localhost:5000/hubs/vehicle-search`

### Expected API Response Format

```typescript
interface VehicleSearchResultDto {
  results: VehicleSearchResult[];
  totalCount: number;
  searchCriteria: any;
}

interface VehicleSearchResult {
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
```

### SignalR Events

The dashboard subscribes to the following SignalR events:

- **Event**: `VehicleSearchUpdate`
- **Payload**: `VehicleSearchResultDto`

## Design Tokens

All spacing, typography, and layout values are defined in `src/app/config/design-tokens.scss` to ensure consistency across the application.

## BEM Naming Convention

All CSS classes follow the BEM (Block Element Modifier) naming convention:

```scss
.block__element--modifier {
  // styles
}
```

Example:
```scss
.home {
  &__hero { }
  &__hero-card { }
  &__cta-button { }
}
```

## License

Copyright (c) Quinntyne Brown. All Rights Reserved.
Licensed under the MIT License.
