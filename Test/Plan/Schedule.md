# Schedule entity test plan

## `ALL` `api/Schedule/*`

- [ScheduleControllerThrowsTooManyRequests()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [ScheduleControllerThrowsUnauthorized()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/Schedule`

- [CreateScheduleReturnsOkStatusCode()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns a schedule when provided with correct data

- [ThrowsErrorWheNameIsTaken()](../Entities/ESchedule/Commands/CreateScheduleCommand.unit.cs) - **unit**  
  Check if returns an error when a taken name is provided

- `DUPLICATE` ~~[CreatesSchedule()](../Entities/ESchedule/Commands/CreateScheduleCommand.unit.cs) - **unit**  
  Check if returns a schedule when provided with correct data~~

## `GET` `api/Schedule`

- [GetAllScheduleReturnsSchedules()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns two schedules when a valid token is provided

- [GetAllScheduleTReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns no schedules when other user's token is provided

- [GetAllScheduleTReturnsOnlyUserMadeSchedules()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns a schedule when valid token is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/ESchedule/Queries/GetSchedulesQuery.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/ESchedule/Queries/GetSchedulesQuery.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/ESchedule/Queries/GetSchedulesQuery.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/ESchedule/Queries/GetSchedulesQuery.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/ESchedule/Queries/GetSchedulesQuery.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters

- `DUPLICATE` ~~[GetsSchedules()](../Entities/ESchedule/Queries/GetSchedulesQuery.unit.cs) - **unit**  
  Check if returns two schedules when a valid token is provided~~

- `DUPLICATE` ~~[ReturnsEmptyWhenWrongUserGetsSchedules()](../Entities/ESchedule/Queries/GetSchedulesQuery.unit.cs) - **unit**  
  Check if returns no schedules when other user's token is provided~~

## `GET` `api/Schedule/{id}`

- [GetScheduleReturnsSchedule()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns a schedule when a valid token is provided

- [GetScheduleThrowsNotFoundWhenWrongIdIsGiven()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when a schedule doesn't exists

- [GetScheduleThrowsNotFoundErrorWhenWrongUserAttemptsGet()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when a schedule is inaccessible for user

- `DUPLICATE` ~~[GetsScheduleWhenIdIsCorrect()](../Entities/ESchedule/Queries/GetScheduleQuery.unit.cs) - **unit**  
  Check if returns a schedule when a valid token is provided~~

## `DELETE` `api/Schedule/{id}`

- [DeleteScheduleReturnsNoContentStatusCode()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns no content when a valid token is provided

- `DUPLICATE` ~~[IsDeleteCalledProperly()](../Entities/ESchedule/Commands/DeleteScheduleCommand.unit.cs) - **unit**  
  Check if returns no content when a valid token is provided~~

## `PATCH` `api/Schedule/{id}`

- [UpdateScheduleReturnsUpdatedSchedule()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an updated schedule when provided with correct data

- [UpdateScheduleThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when schedule doesn't exists

- [UpdateScheduleThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when schedule is inaccessible for user

- [ThrowsErrorWhenURLAlreadyExists()](../Entities/ESchedule/Commands/UpdateScheduleCommand.unit.cs) - **unit**  
  Check if returns an error when url is already taken

- `DUPLICATE` ~~[ReturnsUpdatedUserWhenIdIsCorrect()](../Entities/ESchedule/Commands/UpdateScheduleCommand.unit.cs) - **unit**  
  Check if returns an updated schedule when provided with correct data~~

## `GET` `api/Schedule/byName/{name}`

- [GetScheduleByNameReturnsSchedule()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns a schedule when a valid token is provided

- [GetScheduleByNameThrowsNotFoundWhenWrongIdIsGiven()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when a schedule doesn't exists

- [GetScheduleByNameThrowsNotFoundErrorWhenWrongUserAttemptsGet()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when a schedule is inaccessible for user

- `DUPLICATE` ~~[GetsScheduleWhenNameIsCorrect()](../Entities/ESchedule/Queries/GetScheduleByNameQuery.unit.cs) - **unit**  
  Check if returns a schedule when a valid token is provided~~
