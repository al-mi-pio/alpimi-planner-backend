# Student entity test plan

## `ALL` `api/Student/*`

- [StudentControllerThrowsTooManyRequests()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [StudentSettingsControllerThrowsUnauthorized()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/Student`

- [StudentIsCreated()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if student is created when provided with correct data

- [ThrowsErrorWhenAlbumNumberIsAlreadyTaken](../Entities/EStudent/CreateStudentCommand.unit.cs) - **unit**  
  Check if returns an error when a taken album number is provided

- [ThrowsErrorWhenWrongGroupIdIsGiven()](../Entities/EStudent/CreateStudentCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

## `GET` `api/Student`

- [GetAllStudentsReturnsStudentsFromGroupIfGroupIdIsProvided()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns one student when a valid group id is provided

- [GetAllStudentsReturnsStudentsFromScheduleIfScheduleIsProvided()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns two students when a valid schedule id provided

- [GetAllStudentsReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns no students when other user's token is provided

- [GetAllStudentsReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns no students when wrong id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/EStudent/Queries/GetAllStudent.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/EStudent/Queries/GetAllStudent.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/EStudent/Queries/GetAllStudent.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/EStudent/Queries/GetAllStudent.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/EStudent/Queries/GetAllStudent.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters

## `GET` `api/Student{id}`

- [GetStudentReturnsStudent()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns a student when a valid token is provided

- [GetStudentThrowsNotFoundErrorWhenWrongUserTokenIsGiven()](../Entities/EStudent/StudentController.test.cs) - **integrity** 
  Check if returns an error when a schedule is inaccessible for user

- [GetStudentThrowsNotFoundWhenWrongIdIsGiven()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns no days off when wrong id is provided

## `DELETE` `api/Student/{id}`

- [StudentIsDeleted()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if schedule is deleted when a valid token is provided

## `PATCH` `api/Student/{id}`

- [UpdateStudentReturnsUpdatedStudent()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns an updated day off when provided with correct data

- [updateStudentThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateStudentThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenAlbumNumberIsAlreadyTaken()](../Entities/EStudent/Commands/UpdateStudentCommand.unit.cs) - **unit**  
  Check if returns an error when a taken album number is provided



