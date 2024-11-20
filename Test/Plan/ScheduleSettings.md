# ScheduleSettings entity test plan

## `ALL` `api/ScheduleSettings/*`

- [ScheduleControllerThrowsTooManyRequests()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity**  
  Check if returns an error when request is sent too many times

- [ScheduleSettingsControllerThrowsUnauthorized()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity**  
  Check if returns an error when token is not provided

## `POST` `api/ScheduleSettings`

- [UpdateScheduleSettingsReturnsUpdatedSchedule()](../Entities/EUser/UserController.test.cs) - **integrity**  			
  Check if returns an updated schedule settings when provided with correct data

- [UpdateScheduleSettingsThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EUser/UserController.test.cs) - **integrity**  
  Check if returns an error when schedule settings are inaccessible for user

- [UpdateScheduleSettingsThrowsNotFoundErrorWhenWrongIdIsGiven()](../Entities/EScheduleSettings/ScheduleSettingsController.test.cs) - **integrity**
  Check if returns an error when schedule settings don't exists

- [ThrowsErrorWhenDateIsIncorrect()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **unit**  
  Check if returns an error when incorrect dates are provided


## `GET` `api/User/{scheduleId}`

- [GetScheduleSettingsByScheduleIdReturnsScheduleSettings()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **integrity**
  Check if returns schedule settings when a valid token is provided

- [GetScheduleSettingsByScheduleIdThrowsNotFoundErrorWhenWrongUserAttemptsUpdate()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **integrity**  
  Check if returns an error when schedule settings are inaccessible for user

- [GetUserThrowsNotFoundErrorWhenWrongUserAttemptsGet()](../Entities/EUser/Commands/CreateUserCommand.unit.cs) - **integrity** 	
  Check if returns an error when schedule settings don't exists



