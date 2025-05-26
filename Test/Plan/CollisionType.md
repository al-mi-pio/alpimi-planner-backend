# CollisionType entity test plan

## `ALL` `api/CollisionType/*`

- [CollisionTypeSettingsControllerThrowsUnauthorized()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

- [CollisionTypeControllerThrowsTooManyRequests()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times


## `POST` `api/CollisionType`

- [CollisionTypeIsCreated()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity**  
  Check if collision type is created when provided with correct data

- [ThrowsErrorWhenWrongScheduleIdIsGiven()](../Entities/ECollisionType/CreateCollisionTypeCommand.unit.cs) - **unit** 
  Check if returns an error when incorrect id is provided

- [ThrowsErrorWhenNameIsAlreadyTakenByCollisionType()](../Entities/ECollisionType/CreateCollisionTypeCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by collision type name is provided

- [ThrowsErrorWhenWeightIsLessThan0OrMoreThan1()](../Entities/ECollisionType/Commands/CreateCollisionTypeCommand.unit.cs) - **unit** 
  Check if returns an error when weight is not between 0 and 1


## `DELETE` `api/CollisionType/{id}`

- [CollisionTypeIsDeleted()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity**  
  Check if schedule is deleted when a valid token is provided


## `PATCH` `api/CollisionType/{id}`

- [UpdateCollisionTypeReturnsUpdatedCollisionType()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity**  
  Check if returns an updated day off when provided with correct data

- [UpdateCollisionTypeThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity**  
  Check if returns an error when day off doesn't exists

- [UpdateCollisionTypeThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity**  
  Check if returns an error when day off is inaccessible for user

- [ThrowsErrorWhenNameIsAlreadyTakenByCollisionType()](../Entities/ECollisionType/Commands/UpdateCollisionTypeCommand.unit.cs) - **unit**  
  Check if returns an error when a taken by collision type name is provided

- [ThrowsErrorWhenWeightIsLessThan0OrMoreThan1()](../Entities/ECollisionType/Commands/UpdateCollisionTypeCommand.unit.cs) - **unit** 
  Check if returns an error when weight is not between 0 and 1

## `GET` `api/CollisionType`

- [GetAllCollisionTypesByScheduleReturnsCollisionTypes()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity**  
  Check if returns two collision types when a valid token is provided

- [GetAllCollisionTypesByScheduleReturnsEmptyContentWhenWrongUserAttemptsGet()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity**  
  Check if returns no collision types when other user's token is provided

- [GetAllCollisionTypesByScheduleReturnsEmptyContentWhenWrongIdIsGiven()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity**  
  Check if returns no collision types when wrong schedule id is provided

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/ECollisionType/Queries/GetAllCollisionType.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/ECollisionType/Queries/GetAllCollisionType.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/ECollisionType/Queries/GetAllCollisionType.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/ECollisionType/Queries/GetAllCollisionType.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/ECollisionType/Queries/GetAllCollisionType.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters


## `GET` `api/CollisionType{id}`

- [GetCollisionTypeReturnsCollisionType()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity**  
  Check if returns a collision type when a valid token is provided

- [GetCollisionTypeThrowsNotFoundErrorWhenWrongUserTokenIsGiven()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity** 
  Check if returns an error when a schedule is inaccessible for user

- [GetCollisionTypeThrowsNotFoundWhenWrongIdIsGiven()](../Entities/ECollisionType/CollisionTypeController.test.cs) - **integrity**  
  Check if returns no collision types when wrong id is provided

