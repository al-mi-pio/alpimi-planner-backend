# Auth entity test plan

## `ALL` `api/Auth/*`

- [AuthControllerThrowsTooManyRequests()](../Entities/EAuth/AuthController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

## `POST` `api/Auth/login`

- [LoginReturnsOKStatusCode()](../Entities/EAuth/AuthController.test.cs) - **integrity**  
  Check if returns a token when provided with correct credentials

- [ThrowsErrorWhenIncorrectLoginIsGiven()](../Entities/EAuth/Queries/LoginQuery.unit.cs) - **unit**  
  Check if returns an error when provided with invalid login

- [ThrowsErrorWhenIncorrectPasswordIsGiven()](../Entities/EAuth/Queries/LoginQuery.unit.cs) - **unit**  
  Check if returns an error when provided with invalid password

## `GET` `api/Auth/refresh`

- [RefreshTokenReturnsOKStatusCodeWhenCorrectJWTTokenIsGiven()](../Entities/EAuth/Queries/RefreshTokenQuery.unit.cs) - **integrity**  
  Check if returns a new token when provided with an old one

- [RefreshTokenThrowsUnothorizedErrorWhenNoJWTTokenIsGiven()](../Entities/EAuth/Queries/RefreshTokenQuery.unit.cs) - **integrity**  
  Check if returns an error when token is not provided

