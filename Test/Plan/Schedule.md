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

## `GET` `api/Schedule/{id}`

## `DELETE` `api/Schedule/{id}`

## `PATCH` `api/Schedule/{id}`

## `GET` `api/Schedule/byName/{name}`
