# Student entity test plan

## `ALL` `api/Student/*`

- [StudentControllerThrowsTooManyRequests()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [StudentSettingsControllerThrowsUnauthorized()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/Student`

- [StudentIsCreated()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if student is created when provided with correct data

- [StudentIsCreatedWithSubgroups()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if student is created with subgroups when provided with correct data

- [ThrowsErrorWhenAlbumNumberIsAlreadyTaken](../Entities/EStudent/CreateStudentCommand.unit.cs) - **unit**  
  Check if returns an error when a taken album number is provided

- [ThrowsErrorWhenWrongGroupIdIsGiven()](../Entities/EStudent/CreateStudentCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect group id is provided

- [ThrowsErrorWhenWrongSubgroupIdIsGiven()](../Entities/EStudent/CreateStudentCommand.unit.cs) - **unit**
  Check if returns an error when incorrect subgroup id is provided

- [ThrowsErrorWhenDuplicatedSubgroupIdsAreGiven()](../Entities/EStudent/CreateStudentCommand.unit.cs) - **unit**
  Check if returns an error when duplicated subgroup ids are provided

- [ThrowsErrorWhenGroupIdsFromStudentAndSubgroupDontMatch()](../Entities/EStudent/CreateStudentCommand.unit.cs) - **unit**
  Check if returns an error when student and subgroup are from different groups

## `GET` `api/Student`

- [GetAllStudentsReturnsStudentsFromGroupIfGroupIdIsProvided()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns one student when a valid group id is provided

- [GetAllStudentsReturnsStudentsFromScheduleIfScheduleIdIsProvided()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns two students when a valid schedule id provided

- [GetAllStudentsReturnsStudentsFromSubgroupIfSubgroupIdIsProvided()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns one students when a valid subgroup id provided

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
  Check if returns an updated student when provided with correct data

- [UpdateStudentUpdatesStudentsSubgroups()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if updates students subgroups when provided with correct data

- [updateStudentThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns an error when student doesn't exists

- [UpdateStudentThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EStudent/StudentController.test.cs) - **integrity**  
  Check if returns an error when student is inaccessible for user

- [ThrowsErrorWhenAlbumNumberIsAlreadyTaken()](../Entities/EStudent/Commands/UpdateStudentCommand.unit.cs) - **unit**  
  Check if returns an error when a taken album number is provided

- [ThrowsErrorWhenWrongSubgroupIdIsGiven()](../Entities/EStudent/UpdateStudentCommand.unit.cs) - **unit**
  Check if returns an error when incorrect subgroup id is provided

- [ThrowsErrorWhenDuplicatedSubgroupIdsAreGiven()](../Entities/EStudent/UpdateStudentCommand.unit.cs) - **unit**
  Check if returns an error when duplicated subgroup ids are provided

- [ThrowsErrorWhenGroupIdsFromStudentAndSubgroupDontMatch()](../Entities/EStudent/UpdateStudentCommand.unit.cs) - **unit**
  Check if returns an error when student and subgroup are from different groups



