# Alpimi planner backend

> A WEB API created in .NET for the Alpimi planner  
> _[Test Plan](./Test/Plan/)_

## Requirements

- [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

- [SQL Server Developer](https://www.microsoft.com/pl-pl/sql-server/sql-server-downloads)

- `Recommended` [Visual Studio Community 2022](https://visualstudio.microsoft.com/pl/vs/community/)

## Setup for development

- Launch `View > Server Explorer` in Visual Studio, right click `Data Connections` and `Create New SQL Server Database`:

  - **Server name**: the `hostname` of your computer
  - **Trust server certificate**: `checked`
  - **New database name**: `provide_any_name_here`

- After , right click the database, select `Properties` and copy the connection string

- Create a `.env` file inside the root of the project containing:

```sh
CONNECTION_STRING="paste_your_connection_string_here"
```

- Install Entity Framework tool globally

```sh
dotnet tool install --global dotnet-ef
```

- Update your database with the latest migration

```sh
dotnet ef database update
```

## Setup the test environment

- Create a second database following steps 3 and 4 from the previous `Setup for development` section

- Paste connection string in a new `.runsettings` file like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
    <RunConfiguration>
        <EnvironmentVariables>
            <TEST_CONNECTION_STRING>
                paste_your_connection_string_here
            </TEST_CONNECTION_STRING>
        </EnvironmentVariables>
    </RunConfiguration>
</RunSettings>
```

- Update your new database with the latest migration

```sh
dotnet ef database update --connection "paste_your_connection_string_here"
```

## Tips

- Install [CSharpier](https://marketplace.visualstudio.com/items?itemName=csharpier.CSharpier), go to `Tools > Options > CSharpier > General` and change `Reformat with CSharpier on Save` to `True`. It will save you a lot of time

- Remember to add a migration whenever an entity is changed:

```sh
dotnet ef migrations add MigrationName
dotnet ef database update
```

- And here's how to remove a migration:

```sh
dotnet ef database update PreviousMigrationName
dotnet ef migrations remove
```
