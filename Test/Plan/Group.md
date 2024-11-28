# Group entity test plan

## `ALL` `api/Group/*`

- [GroupControllerThrowsTooManyRequests()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [GroupSettingsControllerThrowsUnauthorized()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/Group`

- [GroupIsCreated()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if group is created when provided with correct data

- [ThrowsErrorWhenNameIsAlreadyTakenByGroup()](../Entities/EGroup/CreateGroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by group name is provided

- [ThrowsErrorWhenWrongScheduleIdIsGiven()](../Entities/EGroup/CreateGroupCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenStudentCountIsLessThan1()](../Entities/EGroup/Commands/CreateGroupCommand.unit.cs) - **unit** 
  Check if returns an error when student count is less than 1

- [ThrowsErrorWhenNameIsAlreadyTakenBySubgroup()](../Entities/EGroup/Commands/CreateGroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by subgroup name is provided


## `GET` `api/Group`

- [GetAllGroupsByScheduleReturnsGroups()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns two groups when a valid token is provided

- [GetAllGroupsByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns no groups when other user's token is provided

- [GetAllGroupsByScheduleReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns no groups when wrong schedule id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/EGroup/Queries/GetAllGroup.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/EGroup/Queries/GetAllGroup.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/EGroup/Queries/GetAllGroup.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/EGroup/Queries/GetAllGroup.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/EGroup/Queries/GetAllGroup.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters

## `GET` `api/Group{id}`

- [GetGroupReturnsGroup()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns a group when a valid token is provided

- [GetGroupThrowsNotFoundErrorWhenWrongUserTokenIsGiven()](../Entities/EGroup/GroupController.test.cs) - **integrity** 
  Check if returns an error when a schedule is inaccessible for user

- [GetGroupThrowsNotFoundWhenWrongIdIsGiven()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns no days off when wrong id is provided

## `DELETE` `api/Group/{id}`

- [GroupIsDeleted()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if schedule is deleted when a valid token is provided

## `PATCH` `api/Group/{id}`

- [UpdateGroupReturnsUpdatedGroup()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns an updated day off when provided with correct data

- [UpdateGroupThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateGroupThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenNameIsAlreadyTakenByGroup()](../Entities/EGroup/Commands/UpdateGroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by group name is provided

- [ThrowsErrorWhenNameIsAlreadyTakenBySubgroup()](../Entities/EGroup/Commands/UpdateGroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by subgroup name is provided

- [ThrowsErrorWhenStudentCountIsLessThan1()](../Entities/EGroup/Commands/UpdateGroupCommand.unit.cs) - **unit** 
  Check if returns an error when student count is less than 1

- [ThrowsErrorWhenStudentCountInGroupIsLessThanSubgroup()](../Entities/EGroup/Commands/UpdateGroupCommand.unit.cs) - **unit** 
  Check if returns an error when updated student count is less student count of a subgroup

