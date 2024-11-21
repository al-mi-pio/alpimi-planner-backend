# ScheduleSettings entity test plan

## `ALL` `api/ScheduleSettings/*`

- [DayOffControllerThrowsTooManyRequests()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [DayOffSettingsControllerThrowsUnauthorized()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/DayOff`

- [DayOffIsCreated()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if dayOff is created when provided with correct data

- [ThrowsErrorWhenWrongDateIsProvided()](../Entities/EDayOff/CreateDayOffCommand.unit.cs) - **unit**  
  Check if returns an error when a wrong date is provided

- [ThrowsErrorWhenWrongScheduleIdIsGiven()](../Entities/EDayOff/CreateDayOffCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

## `POST` `api/DayOff/multiple`

- [MultipleDaysOffAreCreated()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if dayOff is created when provided with correct data

- [ThrowsErrorWhenWrongDateIsProvided()](../Entities/EDayOff/CreateMultipleDayOffCommand.unit.cs) - **unit**  
  Check if returns an error when a wrong date is provided

- [ThrowsErrorWhenWrongScheduleIdIsGiven()](../Entities/EDayOff/CreateMultipleDayOffCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenDateIsIncorrect()](../Entities/EDayOff/CreateMultipleDayOffCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect date is provided


## `GET` `api/DayOff`

- [GGetAllDayOffByScheduleReturnsDaysOff()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns two days off when a valid token is provided

- [GetAllDayOffByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns no days off when other user's token is provided

- [GetAllDayOffByScheduleReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns no days off when wrong schedule id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/ESchedule/Queries/GetAllDayOff.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/ESchedule/Queries/GetAllDayOff.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/ESchedule/Queries/GetAllDayOff.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/ESchedule/Queries/GetAllDayOff.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/ESchedule/Queries/GetAllDayOff.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters

## `DELETE` `api/DayOff/{id}`

- [DayOffIsDeleted()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if schedule is deleted when a valid token is provided

## `PATCH` `api/DayOff/{id}`

- [UpdateDayOffReturnsUpdatedDayOff()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns an updated day off when provided with correct data

- [pdateDayOffThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateDayOffThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWrongDateIsProvided() ](../Entities/EDayOff/Commands/UpdateDayOffCommand.unit.cs) - **unit**  
  Check if returns an error when wrong date is provided


