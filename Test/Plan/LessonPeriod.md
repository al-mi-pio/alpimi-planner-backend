# LessonPeriod entity test plan

## `ALL` `api/LessonPeriod/*`

- [LessonPeriodSettingsControllerThrowsUnauthorized()](../Entities/ELessonPeriod/LessonPeriodController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

- [LessonPeriodControllerThrowsTooManyRequests()](../Entities/ELessonPeriod/LessonPeriodController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times


## `POST` `api/LessonPeriod`

- [LessonPeriodIsCreated()](../Entities/ELessonPeriod/LessonPeriodController.test.cs) - **integrity**  
  Check if LessonPeriod is created when provided with correct data

- [ThrowsErrorWhenWrongScheduleIdIsGiven()](../Entities/ELessonPeriod/CreateLessonPeriodCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenLessonPeriodsOverlap()](../Entities/ELessonPeriod/CreateLessonPeriodCommand.unit.cs) - **unit**  
  Check if returns an error when lesson periods overlap


## `DELETE` `api/LessonPeriod/{id}`

- [LessonPeriodIsDeleted()](../Entities/ELessonPeriod/LessonPeriodController.test.cs) - **integrity**  
  Check if schedule is deleted when a valid token is provided

- 
## `PATCH` `api/LessonPeriod/{id}`

- [UpdateLessonPeriodReturnsUpdatedLessonPeriod()](../Entities/ELessonPeriod/LessonPeriodController.test.cs) - **integrity**  
  Check if returns an updated day off when provided with correct data

- [pdateLessonPeriodThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/ELessonPeriod/LessonPeriodController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateLessonPeriodThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/ELessonPeriod/LessonPeriodController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenLessonPeriodsOverlap()](../Entities/ELessonPeriod/Commands/UpdateLessonPeriodCommand.unit.cs) - **unit**  
  Check if returns an error when lesson periods overlap


## `GET` `api/LessonPeriod`

- [GetAllLessonPeriodByScheduleReturnsLessonPeriods()](../Entities/ELessonPeriod/LessonPeriodController.test.cs) - **integrity**  
  Check if returns two lesson periods when a valid token is provided

- [GetAllLessonPeriodByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/ELessonPeriod/LessonPeriodController.test.cs) - **integrity**  
  Check if returns no lesson periods when other user's token is provided

- [GetAllLessonPeriodByScheduleReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/ELessonPeriod/LessonPeriodController.test.cs) - **integrity**  
  Check if returns no lesson periods when wrong schedule id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/ELessonPeriod/Queries/GetAllLessonPeriod.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/ELessonPeriod/Queries/GetAllLessonPeriod.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/ELessonPeriod/Queries/GetAllLessonPeriod.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/ELessonPeriod/Queries/GetAllLessonPeriod.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/ELessonPeriod/Queries/GetAllLessonPeriod.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters


