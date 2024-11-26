# Group entity test plan

## `ALL` `api/Group/*`

- [GroupControllerThrowsTooManyRequests()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [GroupSettingsControllerThrowsUnauthorized()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/Group`

- [GroupIsCreated()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if group is created when provided with correct data

- [ThrowsErrorWhenNameIsAlreadyTaken()](../Entities/EGroup/CreateGroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken name and surname is provided

- [ThrowsErrorWhenWrongScheduleIdIsGiven()](../Entities/EGroup/CreateGroupCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

-nowy!!!!

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

- [pdateGroupThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateGroupThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EGroup/GroupController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenNameIsAlreadyTaken()](../Entities/EGroup/Commands/UpdateGroupCommand.unit.cs) - **unit**  
  Check if returns an error when out of range date is provided

  --- nowy


