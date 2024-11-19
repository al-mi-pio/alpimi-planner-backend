# Schedule entity test plan

## `POST` `api/Schedule`

- [CreateScheduleReturnsOkStatusCode()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns a schedule when provided with correct data

- [CreateScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

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

- [ThrowsUnothorizedErrorWhenNoTokenIsGiven()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

- [ScheduleControllerThrowsTooManyRequests()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

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

- `DUPLICATE` ~~[GetsSchedules()](../Entities/ESchedule/Queries/GetSchedulesQuery.unit.cs) - **unit**  
  Check if returns two schedules when a valid token is provided~~

## `GET` `api/Schedule/{id}`

- [GetScheduleReturnsSchedule()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns a schedule when a valid token is provided

- [GetScheduleThrowsNotFoundWhenWrongIdIsGiven()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when a schedule doesn't exists

- [GetScheduleThrowsNotFoundErrorWhenWrongUserAttemptsGet()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when a schedule is inaccessible for user

- [GetScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

- `DUPLICATE` ~~[GetsScheduleWhenIdIsCorrect()](../Entities/ESchedule/Queries/GetScheduleQuery.unit.cs) - **unit**  
  Check if returns a schedule when a valid token is provided~~

## `DELETE` `api/Schedule/{id}`

- [DeleteScheduleReturnsNoContentStatusCode()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns no content when a valid token is provided

- [DeleteScheduleThrowsUnothorizedErrorWhenNoTokenIsGiven()](../Entities/ESchedule/ScheduleController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `PATCH` `api/Schedule/{id}`

## `GET` `api/Schedule/byName/{name}`
