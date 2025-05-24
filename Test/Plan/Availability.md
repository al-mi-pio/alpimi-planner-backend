# Availability entity test plan

## `ALL` `api/Availability/*`

- [AvailabilitySettingsControllerThrowsUnauthorized()](../Entities/EAvailability/AvailabilityController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

- [AvailabilityControllerThrowsTooManyRequests()](../Entities/EAvailability/AvailabilityController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times


## `POST` `api/Availability`

- [AvailabilityIsCreated()](../Entities/EAvailability/AvailabilityController.test.cs) - **integrity**  
  Check if availability is created when provided with correct data

- [ThrowsErrorWhenWrongTeacherIdIsGiven()](../Entities/EAvailability/CreateAvailabilityCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect teacher id is provided

- [ThrowsErrorWhenStartIsAfterEnd()](../Entities/EAvailability/Commands/CreateAvailabilityCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect  start and end times are provided

- [ThrowsErrorWhenStartIsLessThan1()](../Entities/EAvailability/Commands/CreateAvailabilityCommand.unit.cs) - **unit** 
  Check if returns an error when start is less than one

- [ThrowsErrorWhenEndIsMoreThanTheAmountOfPeriods()](../Entities/EAvailability/Commands/CreateAvailabilityCommand.unit.cs) - **unit** 
  Check if returns an error when end is more than the amount of  periods

## `DELETE` `api/Availability/{id}`

- [AvailabilityIsDeleted()](../Entities/EAvailability/AvailabilityController.test.cs) - **integrity**  
  Check if availability is deleted when a valid token is provided

## `PATCH` `api/Availability/{id}`

- [UpdateAvailabilityReturnsId()](../Entities/EAvailability/AvailabilityController.test.cs) - **integrity**  
  Check if returns an id when provided with correct data

- [UpdateAvailabilityThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EAvailability/AvailabilityController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateAvailabilityThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EAvailability/AvailabilityController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenWrongTeacherIdIsGiven()](../Entities/EAvailability/UpdateAvailabilityCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect teacher id is provided

- [ThrowsErrorWhenStartIsAfterEnd()](../Entities/EAvailability/Commands/UpdateAvailabilityCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect  start and end times are provided

- [ThrowsErrorWhenStartIsLessThan1()](../Entities/EAvailability/Commands/UpdateAvailabilityCommand.unit.cs) - **unit** 
  Check if returns an error when start is less than one

- [ThrowsErrorWhenEndIsMoreThanTheAmountOfPeriods()](../Entities/EAvailability/Commands/UpdateAvailabilityCommand.unit.cs) - **unit** 
  Check if returns an error when end is more than the amount of periods

## `GET` `api/Availability`

- [GetAllAvailabilitysReturnsAvailabilitysFromTeacherIfTeacherIdIsProvided()](../Entities/EAvailability/AvailabilityController.test.cs) - **integrity**  
  Check if returns availability when a valid teacher id provided

- [GetAllAvailabilitysReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/EAvailability/AvailabilityController.test.cs) - **integrity**  
  Check if returns no availability when other user's token is provided

- [GetAllAvailabilitysReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/EAvailability/AvailabilityController.test.cs) - **integrity**  
  Check if returns no availability when wrong group id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/EAvailability/Queries/GetAllAvailability.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/EAvailability/Queries/GetAllAvailability.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/EAvailability/Queries/GetAllAvailability.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/EAvailability/Queries/GetAllAvailability.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/EAvailability/Queries/GetAllAvailability.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters






