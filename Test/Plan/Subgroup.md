# Subgroup entity test plan

## `ALL` `api/Subgroup/*`

- [SubgroupControllerThrowsTooManyRequests()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [SubgroupSettingsControllerThrowsUnauthorized()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/Subgroup`

- [SubgroupIsCreated()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if subgroup is created when provided with correct data

- [ThrowsErrorWhenNameIsAlreadyTakenBySubgroup()](../Entities/ESubgroup/CreateSubgroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by subgroup name is provided

- [ThrowsErrorWhenWrongGroupIdIsGiven()](../Entities/ESubgroup/CreateSubgroupCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenStudentCountIsLessThan1()](../Entities/ESubgroup/Commands/CreateSubgroupCommand.unit.cs) - **unit** 
  Check if returns an error when student count is less than 1

- [ThrowsErrorWhenNameIsAlreadyTakenByGroup()](../Entities/ESubgroup/Commands/UpdateSubgroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by group name is provided

- [ThrowsErrorWhenStudentCountInSubgroupIsMoreThanGroup()](../Entities/ESubgroup/Commands/UpdateSubgroupCommand.unit.cs) - **unit** 
  Check if returns an error when student count is more than student count of a group

## `GET` `api/Subgroup`

- [GetAllSubgroupsReturnsSubgroupsFromStudentIfStudentIdIsProvided()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns two subgroups when a valid student id provided

- [GetAllSubgroupsReturnsSubgroupsFromGroupIfGroupIdIsProvided()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns two subgroups when a valid group id provided

- [GetAllSubgroupsReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns no subgroups when other user's token is provided

- [GetAllSubgroupsReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns no subgroups when wrong group id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/ESubgroup/Queries/GetAllSubgroup.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/ESubgroup/Queries/GetAllSubgroup.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/ESubgroup/Queries/GetAllSubgroup.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/ESubgroup/Queries/GetAllSubgroup.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/ESubgroup/Queries/GetAllSubgroup.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters

## `GET` `api/Subgroup{id}`

- [GetSubgroupReturnsSubgroup()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns a subgroup when a valid token is provided

- [GetSubgroupThrowsNotFoundErrorWhenWrongUserTokenIsGiven()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity** 
  Check if returns an error when a group is inaccessible for user

- [GetSubgroupThrowsNotFoundWhenWrongIdIsGiven()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns no days off when wrong id is provided

## `DELETE` `api/Subgroup/{id}`

- [SubgroupIsDeleted()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if subgroup is deleted when a valid token is provided

## `PATCH` `api/Subgroup/{id}`

- [UpdateSubgroupReturnsUpdatedSubgroup()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an updated day off when provided with correct data

- [updateSubgroupThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateSubgroupThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenNameIsAlreadyTakenByGroup()](../Entities/ESubgroup/Commands/UpdateSubgroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by group name is provided

- [ThrowsErrorWhenNameIsAlreadyTakenBySubgroup()](../Entities/ESubgroup/Commands/UpdateSubgroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by subgroup name is provided

- [ThrowsErrorWhenStudentCountIsLessThan1()](../Entities/ESubgroup/Commands/UpdateSubgroupCommand.unit.cs) - **unit** 
  Check if returns an error when student count is less than 1

- [ThrowsErrorWhenStudentCountInSubgroupIsMoreThanGroup()](../Entities/ESubgroup/Commands/UpdateSubgroupCommand.unit.cs) - **unit** 
  Check if returns an error when student count is more than student count of a group

