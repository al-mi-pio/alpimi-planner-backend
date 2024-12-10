# ClassroomType entity test plan

## `ALL` `api/ClassroomType/*`

- [ClassroomTypeControllerThrowsTooManyRequests()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [ClassroomTypeSettingsControllerThrowsUnauthorized()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/ClassroomType`

- [ClassroomTypeIsCreated()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if classroom type is created when provided with correct data

- [ThrowsErrorWhenNameIsAlreadyTaken()](../Entities/EClassroomType/CreateClassroomTypeCommand.unit.cs) - **unit**  
  Check if returns an error when a taken name  is provided

- [ThrowsErrorWhenWrongScheduleIdIsGiven()](../Entities/EClassroomType/CreateClassroomTypeCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

## `GET` `api/ClassroomType`

- [GetAllClassroomTypesReturnsClassroomTypesFromScheduleIfScheduleIdIsProvided()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if returns two classroom types when a valid schedule id is provided

- [GetAllClassroomTypesReturnsClassroomTypesFromClassroomIfClassroomIdIsProvided()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if returns one classroom type when a valid classroom id is provided

- [GetAllClassroomTypesReturnsClassroomTypesFromLessonIfLessonIdIsProvided()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if returns one classroom type when a valid lesson id is provided

- [GetAllClassroomTypesReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if returns no classroom types when other user's token is provided

- [GetAllClassroomTypesReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if returns no classroom types when wrong schedule id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/EClassroomType/Queries/GetAllClassroomType.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/EClassroomType/Queries/GetAllClassroomType.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/EClassroomType/Queries/GetAllClassroomType.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/EClassroomType/Queries/GetAllClassroomType.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/EClassroomType/Queries/GetAllClassroomType.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters

## `GET` `api/ClassroomType{id}`

- [GetClassroomTypeReturnsClassroomType()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if returns a classroom type when a valid token is provided

- [GetClassroomTypeThrowsNotFoundErrorWhenWrongUserTokenIsGiven()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity** 
  Check if returns an error when a schedule is inaccessible for user

- [GetClassroomTypeThrowsNotFoundWhenWrongIdIsGiven()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if returns no days off when wrong id is provided

## `DELETE` `api/ClassroomType/{id}`

- [ClassroomTypeIsDeleted()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if schedule is deleted when a valid token is provided

## `PATCH` `api/ClassroomType/{id}`

- [UpdateClassroomTypeReturnsUpdatedClassroomType()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if returns an updated day off when provided with correct data

- [pdateClassroomTypeThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateClassroomTypeThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EClassroomType/ClassroomTypeController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenNameAndSurnameAreAlreadyTaken()](../Entities/EClassroomType/Commands/UpdateClassroomTypeCommand.unit.cs) - **unit**  
  Check if returns an error when out of range date is provided


