# Teacher entity test plan

## `ALL` `api/Teacher/*`

- [TeacherSettingsControllerThrowsUnauthorized()](../Entities/ETeacher/TeacherController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

- [TeacherControllerThrowsTooManyRequests()](../Entities/ETeacher/TeacherController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times


## `POST` `api/Teacher`

- [TeacherIsCreated()](../Entities/ETeacher/TeacherController.test.cs) - **integrity**  
  Check if teacher is created when provided with correct data

- [ThrowsErrorWhenWrongScheduleIdIsGiven()](../Entities/ETeacher/CreateTeacherCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenNameAndSurnameAreAlreadyTaken()](../Entities/ETeacher/CreateTeacherCommand.unit.cs) - **unit**  
  Check if returns an error when a taken name and surname is provided


## `DELETE` `api/Teacher/{id}`

- [TeacherIsDeleted()](../Entities/ETeacher/TeacherController.test.cs) - **integrity**  
  Check if schedule is deleted when a valid token is provided


## `PATCH` `api/Teacher/{id}`

- [UpdateTeacherReturnsUpdatedTeacher()](../Entities/ETeacher/TeacherController.test.cs) - **integrity**  
  Check if returns an updated day off when provided with correct data

- [pdateTeacherThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/ETeacher/TeacherController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateTeacherThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/ETeacher/TeacherController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenNameAndSurnameAreAlreadyTaken()](../Entities/ETeacher/Commands/UpdateTeacherCommand.unit.cs) - **unit**  
  Check if returns an error when out of range date is provided


## `GET` `api/Teacher`

- [GetAllTeachersByScheduleReturnsTeachers()](../Entities/ETeacher/TeacherController.test.cs) - **integrity**  
  Check if returns two teachers when a valid token is provided

- [GetAllTeachersByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/ETeacher/TeacherController.test.cs) - **integrity**  
  Check if returns no teachers when other user's token is provided

- [GetAllTeachersByScheduleReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/ETeacher/TeacherController.test.cs) - **integrity**  
  Check if returns no teachers when wrong schedule id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/ETeacher/Queries/GetAllTeacher.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/ETeacher/Queries/GetAllTeacher.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/ETeacher/Queries/GetAllTeacher.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/ETeacher/Queries/GetAllTeacher.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/ETeacher/Queries/GetAllTeacher.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters


## `GET` `api/Teacher{id}`

- [GetTeacherReturnsTeacher()](../Entities/ETeacher/TeacherController.test.cs) - **integrity**  
  Check if returns a teacher when a valid token is provided

- [GetTeacherThrowsNotFoundErrorWhenWrongUserTokenIsGiven()](../Entities/ETeacher/TeacherController.test.cs) - **integrity** 
  Check if returns an error when a schedule is inaccessible for user

- [GetTeacherThrowsNotFoundWhenWrongIdIsGiven()](../Entities/ETeacher/TeacherController.test.cs) - **integrity**  
  Check if returns no days off when wrong id is provided


