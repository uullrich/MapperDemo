# DIRS21 Mapping System Documentation

This document provides comprehensive information about the DIRS21 Mapping System, including its architecture, usage guidelines, and how to extend it for new mapping scenarios.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Usage](#usage)
- [Extending the System](#extending-the-system)
- [Error Handling](#error-handling)
- [Best Practices](#best-practices)
- [Limitations](#limitations)

## Overview

The DIRS21 Mapping System is designed to convert data between DIRS21's internal data models and various partner-specific formats. The system employs a flexible, extensible architecture that allows for easy addition of new data models and mapping rules without modifying the core mapping functionality.

## Architecture

The mapping system is built around several core components:

- **IMapHandler**: The main entry point for mapping operations, exposing the `Map` method.
- **MapHandler**: The concrete implementation of `IMapHandler` that orchestrates the mapping process.
- **IMapper**: The interface that all mapper implementations must implement.
- **BaseMapper<TSource, TTarget>**: A base class for creating strongly-typed mappers.
- **Concrete Mapper Implementations**: Classes that inherit from `BaseMapper` and implement the mapping logic for specific source and target types.

The system uses the Strategy Pattern to select the appropriate mapper for a given source and target type pair, and Dependency Injection to provide extensibility.

## Usage

### Basic Usage

```csharp
using Microsoft.Extensions.DependencyInjection;
using MapperDemo.Core;
using MapperDemo.Models.DIRS21;
using Google = MapperDemo.Models.Google;

// Get the map handler (from DI container)
var mapHandler = serviceProvider.GetRequiredService<IMapHandler>();

// Map from DIRS21 Reservation to Google Reservation
Google.Reservation googleReservation = mapHandler.Map<Google.Reservation>(
    dirs21Reservation,
    "Model.Reservation",
    "Google.Reservation");

// Map from Google Reservation back to DIRS21 Reservation
Reservation mappedBack = mapHandler.Map<Reservation>(
    googleReservation,
    "Google.Reservation",
    "Model.Reservation");
```

### Setting Up with Dependency Injection

```csharp
// In the service registration code
services.AddMappingSystem()
    .AddMapper<DIRS21ToGoogleReservationMapper>()
    .AddMapper<GoogleToDIRS21ReservationMapper>();
```

## Extending the System

### Creating a New Mapper

1. **Create the data models** (if they don't already exist)

```csharp
// For example, adding a new partner "Booking.com"
namespace MapperDemo.Models.Booking
{
    public class Reservation
    {
        public string BookingId { get; set; }
        public string ArrivalDate { get; set; }
        public string DepartureDate { get; set; }
        // Other properties...
    }

    // Other model classes...
}
```

2. **Create a mapper from DIRS21 to the new partner format**

```csharp
public class DIRS21ToBookingReservationMapper : BaseMapper<DIRS21.Reservation, Booking.Reservation>
{
    public DIRS21ToBookingReservationMapper()
        : base("Model.Reservation", "Booking.Reservation")
    {
    }

    public override Booking.Reservation Map(DIRS21.Reservation source)
    {
        // Implement mapping logic here
        return new Booking.Reservation
        {
            BookingId = source.Id.ToString(),
            ArrivalDate = source.CheckInDate?.ToString("yyyy-MM-dd"),
            DepartureDate = source.CheckOutDate?.ToString("yyyy-MM-dd"),
            // Map other properties...
        };
    }
}
```

3. **Create a mapper from the new partner format to DIRS21**

```csharp
public class BookingToDIRS21ReservationMapper : BaseMapper<Booking.Reservation, DIRS21.Reservation>
{
    public BookingToDIRS21ReservationMapper()
        : base("Booking.Reservation", "Model.Reservation")
    {
    }

    public override DIRS21.Reservation Map(Booking.Reservation source)
    {
        // Implement mapping logic here
        var result = new DIRS21.Reservation
        {
            Id = Guid.Parse(source.BookingId),
            CheckInDate = DateTime.Parse(source.ArrivalDate),
            CheckOutDate = DateTime.Parse(source.DepartureDate),
            // Map other properties...
        };

        return result;
    }
}
```

4. **Register the new mappers**

```csharp
services.AddMappingSystem()
    .AddMapper<DIRS21ToBookingReservationMapper>()
    .AddMapper<BookingToDIRS21ReservationMapper>();
```

### Supporting New Entity Types

The same pattern applies when adding new entity types (e.g., Room, Payment, etc.):

1. Create the data models
2. Create mappers in both directions
3. Register the mappers

## Error Handling

The mapping system includes built-in error handling to address common mapping issues:

- **No Mapper Found**: When no mapper is available for the requested source and target types
- **Invalid Source Type**: When the provided source object doesn't match the expected type
- **Mapping Errors**: When exceptions occur during the mapping process

All mapping errors are wrapped in a `MappingException` that provides context about the source and target types.

### Error Handling Example

```csharp
try
{
    var result = mapHandler.Map(data, sourceType, targetType);
    // Process the result
}
catch (MappingException ex) when (ex.SourceType == "Model.Reservation" && ex.TargetType == "Google.Reservation")
{
    // Handle missing mapper with context
    logger.LogWarning("Missing mapper: {Source}->{Target}", ex.SourceType, ex.TargetType);
}
catch (MappingException ex)
{
    // Handle other mapping errors
    logger.LogError(ex, "Mapping error occurred");
}
```

## Best Practices

1. **Create Dedicated Model Classes**: Don't try to map directly to/from partner API DTOs. Create dedicated model classes that represent the exact contract expected by partners.

2. **Keep Mappers Focused**: Each mapper should handle one specific source-target pair.

3. **Handle Edge Cases**: Always consider null values, missing data, and format differences.

4. **Add Validation**: Validate data before and after mapping to ensure data integrity.

5. **Use Meaningful Type Names**: Choose clear, descriptive type names (e.g., "Model.Reservation", "Google.Reservation") that make the mapping relationship obvious.
