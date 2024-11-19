# User entity test plan

## `ALL` `api/User/*`

- [UserControllerThrowsTooManyRequests()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [UserControllerThrowsUnauthorized()](../Entities/UserController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/User`

- [CreateUSerReturnsOkStatusCode()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns a user when provided with correct data

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

- `DUPLICATE` ~~[CreatesUserWhenPaswordIsCorrect()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **unit**  
  Check if returns a user when provided with correct data~~

## `GET` `api/User/{id}`

- [GetUserReturnsUser()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns a user when a valid token is provided

- [GetUserThrowsNotFoundWhenWrongIdIsGiven()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when a user doesn't exists

- [GetUserThrowsNotFoundErrorWhenWrongUserAttemptsGet()](../Entities/UserController.test.cs) - **integrity**  
  Check if returns an error when a user is inaccessible for user

- `DUPLICATE` ~~[GetsUserWhenIdIsCorrect()](../Entities/EUser/Queries/GetUserQuery.unit.cs) - **unit**  
  Check if returns a user when a valid token is provided~~

## `DELETE` `api/User/{id}`

- [DeleteUserReturnsNoContentStatusCode()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns no content when a valid token is provided

- `DUPLICATE` ~~[IsDeleteCalledProperly()](../Entities/EUser/Commands/DeleteUserCommand.unit.cs) - **unit**  
  Check if returns no content when a valid token is provided~~

## `PATCH` `api/User/{id}`

- [UpdateUserReturnsUpdatedUser()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an updated user when provided with correct data

- [UpdateUserThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when user doesn't exists

- [UpdateUserThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when user is inaccessible for the user

- [ThrowsErrorWhenURLAlreadyExists()](../Entities/EUser/Commands/UpdateUserCommand.unit.cs) - **unit**  
  Check if returns an error when url is already taken

- `DUPLICATE` ~~[ReturnsUpdatedUserWhenIdIsCorrect()](../Entities/EUser/Commands/UpdateUserCommand.unit.cs) - **unit**  
  Check if returns an updated User when provided with correct data~~

## `GET` `api/User/byLogin/{login}`

- [GetUserByNameReturnsUser()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns a user when a valid token is provided

- [GetUserByNameThrowsNotFoundWhenWrongIdIsGiven()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when a user doesn't exists

- [GetUserByNameThrowsNotFoundErrorWhenWrongUserAttemptsGet()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when a user is inaccessible for the user

- `DUPLICATE` ~~[GetsUserWhenNameIsCorrect()](../Entities/EUser/Queries/GetUserByNameQuery.unit.cs) - **unit**  
  Check if returns a user when a valid token is provided~~
