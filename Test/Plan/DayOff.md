# DayOff entity test plan

## `ALL` `api/DayOff/*`

- [DayOffControllerThrowsTooManyRequests()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [DayOffSettingsControllerThrowsUnauthorized()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/DayOff`

- [DayOffIsCreated()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if dayOff is created when provided with correct data

- [ThrowsErrorWhenOutOfRangeDateIsProvided()](../Entities/EDayOff/CreateDayOffCommand.unit.cs) - **unit**  
  Check if returns an error when a out of range date is provided

- [ThrowsErrorWhenWrongScheduleIdIsGiven()](../Entities/EDayOff/CreateDayOffCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenNumberOfDaysIsIncorrect()](../Entities/EDayOff/CreateDayOffCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect number of days is provided

## `GET` `api/DayOff`

- [GetAllDayOffByScheduleReturnsDaysOff()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns two days off when a valid token is provided

- [GetAllDayOffByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns no days off when other user's token is provided

- [GetAllDayOffByScheduleReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/EDayOff/DayOffController.test.cs) - **integrity**  
  Check if returns no days off when wrong schedule id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/EDayOff/Queries/GetAllDayOff.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/EDayOff/Queries/GetAllDayOff.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/EDayOff/Queries/GetAllDayOff.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/EDayOff/Queries/GetAllDayOff.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/EDayOff/Queries/GetAllDayOff.unit.cs) - **unit**  
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

- [ ThrowsErrorWhenOutOfRangeDateIsProvided()](../Entities/EDayOff/Commands/UpdateDayOffCommand.unit.cs) - **unit**  
  Check if returns an error when out of range date is provided

- [ThrowsErrorWhenDateStartIsAfterDateEnd()](../Entities/EDayOff/Commands/UpdateDayOffCommand.unit.cs) - **unit** 
  Check if returns an error when date start is after date end  


