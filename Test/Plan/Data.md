# Import/Export data test plan

## `ALL` `api/Data/*`

- [DataControllerThrowsUnauthorized()](../Entities/EData/FataController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

- [DataControllerThrowsTooManyRequests()](../Entities/EData/DataController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times


## `POST` `api/Data/import`

- [ImportDataReturnsMultipleBadImportDataErrors()](../Entities/EData/Commands/ImportDataCommand.unit.cs) - **unit**
  Check if returns multiple errors when bad import data is given

- [ImportDataCorrectlyImportsData()](../Entities/EData/DataController.test.cs) **integrity**
  Check if correctly imports data

- [ImportDataReturnsErrorsWhenBadReferencesAreGiven()](../Entities/EData/Commands/ImportDataCommand.unit.cs) - **unit**
  Check if returns errors when a bad regerences to a entity is given

## 'GET' 'api/Data/export/{scheduleId}'
- [ExportDataCorrectlyExportsData()](../Entities/EData/DataController.test.cs) **integrity**
  Check if correctly exports data