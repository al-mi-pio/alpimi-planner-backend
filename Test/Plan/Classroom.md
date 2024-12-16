# Classroom entity test plan

## `ALL` `api/Classroom/*`

- [ClassroomSettingsControllerThrowsUnauthorized()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

- [ClassroomControllerThrowsTooManyRequests()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times


## `POST` `api/Classroom`

- [ClassroomIsCreated()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if classroom is created when provided with correct data

- [ClassroomIsCreatedWithClassroomTypes()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if classroom is created with classroom types when provided with correct data

- [ThrowsErrorWhenWrongScheduleIdIsGiven()](../Entities/EClassroom/CreateClassroomCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenNameIsAlreadyTaken()](../Entities/EClassroom/CreateClassroomCommand.unit.cs) - **unit**  
  Check if returns an error when a taken name  is provided

- [ThrowsErrorWhenCapacityIsLessThan1](../Entities/EClassroom/CreateClassroomCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect capacity is provided

- [ThrowsErrorWhenDuplicatedClassroomTypeIdsAreGiven()](../Entities/EClassroom/CreateClassroomCommand.unit.cs) - **unit** 			
  Check if returns an error when duplicated classroom type ids are provided

- [ThrowsErrorWhenWrongClassroomTypeIdIsGiven()](../Entities/EClassroom/CreateClassroomCommand.unit.cs) - **unit** 			
  Check if returns an error when incorrect classroom type id is provided

- [ThrowsErrorWhenScheduleIdsFromClassroomAndClassroomTypeDontMatch()](../Entities/EClassroom/CreateClassroomCommand.unit.cs) - **unit**
  Check if returns an error when classroom and classroom type are from different groups


## `DELETE` `api/Classroom/{id}`

- [ClassroomIsDeleted()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if schedule is deleted when a valid token is provided


## `PATCH` `api/Classroom/{id}`

- [UpdateClassroomReturnsUpdatedClassroom()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if returns an updated day off when provided with correct data

- [UpdateClassroomUpdatesClassroomsClassroomTypes()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if updates classrooms classroom types when provided with correct data

- [UpdateClassroomThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateClassroomThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenNameAndSurnameAreAlreadyTaken()](../Entities/EClassroom/Commands/UpdateClassroomCommand.unit.cs) - **unit**  
  Check if returns an error when out of range date is provided

- [ThrowsErrorWhenCapacityIsLessThan1](../Entities/EClassroom/UpdateClassroomCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect capacity is provided

- [ThrowsErrorWhenDuplicatedClassroomTypeIdsAreGiven()](../Entities/EClassroom/UpdateClassroomCommand.unit.cs) - **unit** 			
  Check if returns an error when duplicated classroom type ids are provided

- [ThrowsErrorWhenWrongClassroomTypeIdIsGiven()](../Entities/EClassroom/UpdateClassroomCommand.unit.cs) - **unit** 			
  Check if returns an error when incorrect classroom type id is provided

- [ThrowsErrorWhenScheduleIdsFromClassroomAndClassroomTypeDontMatch()](../Entities/EClassroom/UpdateClassroomCommand.unit.cs) - **unit**
  Check if returns an error when classroom and classroom type are from different groups


## `GET` `api/Classroom`

- [GetAllClassroomsReturnsClassroomsFromScheduleIfShceduleIdIsProvided()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if returns two classrooms when a valid token is provided

- [GetAllClassroomsReturnsClassroomsFromClassroomTypeIfClassroomTypeIdIsProvided()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if returns two classrooms when a valid token is provided

- [GetAllClassroomsReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if returns no classrooms when other user's token is provided

- [GetAllClassroomsReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if returns no classrooms when wrong schedule id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/EClassroom/Queries/GetAllClassroom.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/EClassroom/Queries/GetAllClassroom.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/EClassroom/Queries/GetAllClassroom.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/EClassroom/Queries/GetAllClassroom.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/EClassroom/Queries/GetAllClassroom.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters


## `GET` `api/Classroom{id}`

- [GetClassroomReturnsClassroom()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if returns a classroom when a valid token is provided

- [GetClassroomThrowsNotFoundWhenWrongIdIsGiven()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity**  
  Check if returns no days off when wrong id is provided

- [GetClassroomThrowsNotFoundErrorWhenWrongUserTokenIsGiven()](../Entities/EClassroom/ClassroomController.test.cs) - **integrity** 
  Check if returns an error when a schedule is inaccessible for user

