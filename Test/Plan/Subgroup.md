# Subgroup entity test plan

## `ALL` `api/Subgroup/*`

- [SubgroupSettingsControllerThrowsUnauthorized()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

- [SubgroupControllerThrowsTooManyRequests()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times


## `POST` `api/Subgroup`

- [SubgroupIsCreated()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if subgroup is created when provided with correct data

- [ThrowsErrorWhenWrongGroupIdIsGiven()](../Entities/ESubgroup/CreateSubgroupCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenNameIsAlreadyTakenBySubgroup()](../Entities/ESubgroup/CreateSubgroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by subgroup name is provided

- [ThrowsErrorWhenNameIsAlreadyTakenByGroup()](../Entities/ESubgroup/Commands/UpdateSubgroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by group name is provided

- [ThrowsErrorWhenStudentCountIsLessThan1()](../Entities/ESubgroup/Commands/CreateSubgroupCommand.unit.cs) - **unit** 
  Check if returns an error when student count is less than 1

- [ThrowsErrorWhenStudentCountInSubgroupIsMoreThanGroup()](../Entities/ESubgroup/Commands/UpdateSubgroupCommand.unit.cs) - **unit** 
  Check if returns an error when student count is more than student count of a group


## `DELETE` `api/Subgroup/{id}`

- [SubgroupIsDeleted()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if subgroup is deleted when a valid token is provided

- [SubgroupsLessonsAreDeleted](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if lessons of the subgroup are deleted


## `PATCH` `api/Subgroup/{id}`

- [UpdateSubgroupReturnsUpdatedSubgroup()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an updated subgroup when provided with correct data

- [UpdateSubgroupThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an error when subgroup doesn't exists

- [UpdateSubgroupThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an error when subgroup is inaccessible for user

- [ThrowsErrorWhenNameIsAlreadyTakenBySubgroup()](../Entities/ESubgroup/Commands/UpdateSubgroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by subgroup name is provided

- [ThrowsErrorWhenNameIsAlreadyTakenByGroup()](../Entities/ESubgroup/Commands/UpdateSubgroupCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by group name is provided

- [ThrowsErrorWhenStudentCountIsLessThan1()](../Entities/ESubgroup/Commands/UpdateSubgroupCommand.unit.cs) - **unit** 
  Check if returns an error when student count is less than 1

- [ThrowsErrorWhenStudentCountInSubgroupIsMoreThanGroup()](../Entities/ESubgroup/Commands/UpdateSubgroupCommand.unit.cs) - **unit** 
  Check if returns an error when student count is more than student count of a group

## `PATCH` `api/Subgroup/join`

- [JoinSubgroupReturnsJointSubgroupId()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an joint subgroup Id

- [JoinSubgroupThrowsErrorWhenWrongIdIsGiven()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an error when atleast one of the subgroups doesn't exists

- [JoinSubgroupThrowsErrorWhenWrongUserAttemptsUpdate()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns an error when atleast one of the subgroups is inaccessible for user

- [ThrowsErrorWhenDuplicatedSubgroupIdsAreGiven()](../Entities/ESubgroup/Commands/JoinSubgroupCommand.unit.cs) - **unit**  
  Check if returns an error when duplicated subgroup ids are provided

- [ThrowsErrorWhenWrongSubgroupIdIsGiven()](../Entities/ESubgroup/Commands/JoinSubgroupCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect subgroup id is provided


## `GET` `api/Subgroup`

- [GetAllSubgroupsReturnsSubgroupsFromStudentIfStudentIdIsProvided()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns two subgroups when a valid student id provided

- [GetAllSubgroupsReturnsSubgroupsFromGroupIfGroupIdIsProvided()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns two subgroups when a valid group id provided

- [GetAllSubgroupsReturnsSubgroupsFromJointSubgroupIfJointSubgroupIdIsProvided()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**  
  Check if returns two subgroups when a valid joint subgroup id provided

- [GetAllSubgroupsReturnsSubgroupsFromPublicSchedules()](../Entities/ESubgroup/SubgroupController.test.cs) - **integrity**   
  Check if returns two classrooms when no token is provided

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


