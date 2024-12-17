# LessonType entity test plan

## `ALL` `api/LessonType/*`

- [LessonTypeSettingsControllerThrowsUnauthorized()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

- [LessonTypeControllerThrowsTooManyRequests()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times


## `POST` `api/LessonType`

- [LessonTypeIsCreated()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if lesson type is created when provided with correct data

- [ThrowsErrorWhenWrongScheduleIdIsGiven()](../Entities/ELessonType/CreateLessonTypeCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenNameIsAlreadyTaken()](../Entities/ELessonType/CreateLessonTypeCommand.unit.cs) - **unit**  
  Check if returns an error when a taken name  is provided

- [ThrowsErrorWhenColorIsLessThan0](../Entities/ELessonType/CreateLessonTypeCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect color value is provided

- [ThrowsErrorWhenColorIsMoreThan359](../Entities/ELessonType/CreateLessonTypeCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect color value is provided


## `DELETE` `api/LessonType/{id}`

- [LessonTypeIsDeleted()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if schedule is deleted when a valid token is provided


## `PATCH` `api/LessonType/{id}`

- [UpdateLessonTypeReturnsUpdatedLessonType()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns an updated lesson type when provided with correct data

- [UpdateLessonTypeThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns an error when lesson type is inaccessible for user

- [pdateLessonTypeThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns an error when lesson type doesn't exists

- [ThrowsErrorWhenNameAndSurnameAreAlreadyTaken()](../Entities/ELessonType/Commands/UpdateLessonTypeCommand.unit.cs) - **unit**  
  Check if returns an error when out of range date is provided

- [ThrowsErrorWhenColorIsLessThan0](../Entities/ELessonType/CreateLessonTypeCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect color value is provided

- [ThrowsErrorWhenColorIsMoreThan359](../Entities/ELessonType/CreateLessonTypeCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect color value is provided


## `GET` `api/LessonType`

- [GetAllLessonTypesReturnsLessonTypes()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns two lesson types when a valid token is provided

- [GetAllLessonTypesReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
  Check if returns no lesson types when other user's token is provided

- [GetAllLessonTypesReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/ELessonType/LessonTypeController.test.cs) - **integrity**  
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


