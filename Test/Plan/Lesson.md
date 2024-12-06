# Lesson entity test plan

## `ALL` `api/Lesson/*`

- [LessonControllerThrowsTooManyRequests()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [LessonSettingsControllerThrowsUnauthorized()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/Lesson`

- [LessonIsCreated()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if lesson is created when provided with correct data

- [ThrowsErrorWhenNameIsAlreadyTakenByLesson()](../Entities/ELesson/CreateLessonCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by lesson name is provided

- [ThrowsErrorWhenWrongLessonTypeIdIsGiven()](../Entities/ELesson/CreateLessonCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenWrongSubgroupIdIsGiven()](../Entities/ELesson/CreateLessonCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenAmountOfHoursIsLessThan1()](../Entities/ELesson/Commands/CreateLessonCommand.unit.cs) - **unit** 
  Check if returns an error when student count is less than 1


## `GET` `api/Lesson`

- [GetAllLessonsReturnsLessonsFromGroupIfGroupIdIsProvided()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if returns two lessons when a valid student id provided

- [GetAllLessonsReturnsLessonsFromSubgroupIfSubgroupIdIsProvided()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if returns two lessons when a valid group id provided

- [GetAllLessonsReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if returns no lessons when other user's token is provided

- [GetAllLessonsReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if returns no lessons when wrong group id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/ELesson/Queries/GetAllLesson.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/ELesson/Queries/GetAllLesson.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/ELesson/Queries/GetAllLesson.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/ELesson/Queries/GetAllLesson.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/ELesson/Queries/GetAllLesson.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters

## `GET` `api/Lesson{id}`

- [GetLessonReturnsLesson()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if returns a lesson when a valid token is provided

- [GetLessonThrowsNotFoundErrorWhenWrongUserTokenIsGiven()](../Entities/ELesson/LessonController.test.cs) - **integrity** 
  Check if returns an error when a group is inaccessible for user

- [GetLessonThrowsNotFoundWhenWrongIdIsGiven()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if returns no days off when wrong id is provided

## `DELETE` `api/Lesson/{id}`

- [LessonIsDeleted()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if lesson is deleted when a valid token is provided

## `PATCH` `api/Lesson/{id}`

- [UpdateLessonReturnsUpdatedLesson()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if returns an updated day off when provided with correct data

- [updateLessonThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateLessonThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/ELesson/LessonController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenNameIsAlreadyTakenByLesson()](../Entities/ELesson/Commands/UpdateLessonCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by lesson name is provided

- [ThrowsErrorWhenAmountOfHoursIsLessThan1()](../Entities/ELesson/Commands/UpdateLessonCommand.unit.cs) - **unit** 
  Check if returns an error when student count is less than 1



