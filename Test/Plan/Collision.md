# Collision entity test plan

## `GET` `api/Collision`

- [ThrowsErrorWhenIncorrectPerPageIsGiven()](../Entities/ECollision/Queries/GetAllCollision.unit.cs) - **unit**  
  Check if returns an error when provided with invalid perPage

- [ThrowsErrorWhenIncorrectPageIsGiven()](../Entities/ECollision/Queries/GetAllCollision.unit.cs) - **unit**  
  Check if returns an error when provided with invalid page

- [ThrowsErrorWhenIncorrectSortByIsGiven()](../Entities/ECollision/Queries/GetAllCollision.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortBy

- [ThrowsErrorWhenIncorrectSortOrderIsGiven()](../Entities/ECollision/Queries/GetAllCollision.unit.cs) - **unit**  
  Check if returns an error when provided with invalid sortOrder

- [ThrowsMultipleErrorMessages()](../Entities/ECollision/Queries/GetAllCollision.unit.cs) - **unit**  
  Check if returns multiple errors when provided with multiple invalid parameters
