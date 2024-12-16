# User entity test plan

## `ALL` `api/User/*`

- [UserControllerThrowsUnauthorized()](../Entities/UserController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

- [UserControllerThrowsTooManyRequests()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times


## `POST` `api/User`

- [UserIsCreated()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if user is created when provided with correct data

- [CreateUserThrowsForbiddenErrorWhenWrongTokenIsGiven()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns forbidden when a invalid token is provided

- [ThrowsErrorWhenPasswordIsTooShort()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **unit**  
  Check if returns an error when provided with too short password

- [ThrowsErrorWhenPasswordIsTooLong()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **unit**  
  Check if returns an error when provided with too long password

- [ThrowsErrorWhenPasswordDosentContainSmallLetters()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **unit**  
  Check if returns an error when provided with no small letters in password

- [ThrowsErrorWhenPasswordDosentContainBigLetters()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **unit**  
  Check if returns an error when provided with no big letters in password

- [ThrowsErrorWhenPasswordDosentContainSymbols()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **unit**  
  Check if returns an error when provided with no symbols in password

- [ThrowsErrorWhenPasswordDosentContainDigits()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **unit**  
  Check if returns an error when provided with no digits in password

- [ThrowsErrorWhenLoginAlreadyExists()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **unit**  
  Check if returns an error when login already exists

- [ThrowsErrorWhenURLAlreadyExists()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **unit**  
  Check if returns an error when url already exists

- [ThrowsMultipleErrorMessages()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters


## `DELETE` `api/User/{id}`

- [UserIsDeleted()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if user is deleted when a valid token is provided

- [DeleteUserThrowsForbiddenErrorWhenNoTokenIsGiven()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns forbidden when a invalid token is provided


## `PATCH` `api/User/{id}`

- [UpdateUserReturnsUpdatedUser()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an updated user when provided with correct data

- [UpdateUserThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when user doesn't exists

- [UpdateUserThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when user is inaccessible for the user

- [ThrowsErrorWhenURLAlreadyExists()](../Entities/EUser/Commands/UpdateUserCommand.unit.cs) - **unit**  
  Check if returns an error when url is already taken


## `GET` `api/User/{id}`

- [GetUserReturnsUser()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns a user when a valid token is provided

- [GetUserThrowsNotFoundErrorWhenWrongUserAttemptsGet()](../Entities/UserController.test.cs) - **integrity**  
  Check if returns an error when a user is inaccessible for user

- [GetUserThrowsNotFoundWhenWrongIdIsGiven()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when a user doesn't exists


## `GET` `api/User/byLogin/{login}`

- [GetUserByNameReturnsUser()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns a user when a valid token is provided

- [GetUserByNameThrowsNotFoundErrorWhenWrongUserAttemptsGet()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when a user is inaccessible for the user

- [GetUserByNameThrowsNotFoundWhenWrongLoginIsGiven()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when a user doesn't exists




