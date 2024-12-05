# LessonType entity test plan

## `ALL` `api/LessonType/*`

- [LessonTypeControllerThrowsTooManyRequests()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [LessonTypeSettingsControllerThrowsUnauthorized()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/LessonType`

- [LessonTypeIsCreated()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if lesson type is created when provided with correct data

- [ThrowsErrorWhenNameIsAlreadyTaken()](../Entities/ELessonType/CreateLessonTypeCommand.unit.cs) - **unit**  
  Check if returns an error when a taken name  is provided

- [ThrowsErrorWhenWrongScheduleIdIsGiven()](../Entities/ELessonType/CreateLessonTypeCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenColorIsLessThan1](../Entities/ELessonType/CreateLessonTypeCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect capacity is provided

## `GET` `api/LessonType`

- [GetAllLessonTypesByScheduleReturnsLessonTypes()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns two lesson types when a valid token is provided

- [GetAllLessonTypesByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns no lesson types when other user's token is provided

- [GetAllLessonTypesByScheduleReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns no lesson types when wrong schedule id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/ELessonType/Queries/GetAllLessonType.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/ELessonType/Queries/GetAllLessonType.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/ELessonType/Queries/GetAllLessonType.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/ELessonType/Queries/GetAllLessonType.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/ELessonType/Queries/GetAllLessonType.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters

## `GET` `api/LessonType{id}`

- [GetLessonTypeReturnsLessonType()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns a lesson type when a valid token is provided

- [GetLessonTypeThrowsNotFoundErrorWhenWrongUserTokenIsGiven()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity** 
  Check if returns an error when a schedule is inaccessible for user

- [GetLessonTypeThrowsNotFoundWhenWrongIdIsGiven()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns no days off when wrong id is provided

## `DELETE` `api/LessonType/{id}`

- [LessonTypeIsDeleted()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if schedule is deleted when a valid token is provided

## `PATCH` `api/LessonType/{id}`

- [UpdateLessonTypeReturnsUpdatedLessonType()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns an updated day off when provided with correct data

- [pdateLessonTypeThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateLessonTypeThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenNameAndSurnameAreAlreadyTaken()](../Entities/ELessonType/Commands/UpdateLessonTypeCommand.unit.cs) - **unit**  
  Check if returns an error when out of range date is provided

- [ThrowsErrorWhenColorIsLessThan1](../Entities/ELessonType/UpdateLessonTypeCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect capacity is provided


