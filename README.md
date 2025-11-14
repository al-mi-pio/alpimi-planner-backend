# Alpimi planner backend

> A WEB API created in .NET for the Alpimi planner  
> _[Test Plan](./Test/Plan/)_

## Requirements

- [.NET SDK 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

- [SQL Server Developer](https://www.microsoft.com/pl-pl/sql-server/sql-server-downloads)

- `Recommended` [Visual Studio Community 2026](https://visualstudio.microsoft.com/pl/vs/community/)

## Setup for development

- Launch `View > Server Explorer` in Visual Studio, right click `Data Connections` and `Create New SQL Server Database`:

  - **Server name**: the `hostname` of your computer
  - **Trust server certificate**: `checked`
  - **New database name**: `provide_any_name_here`

- After a successful connection, right click the database, select `Properties` and copy the connection string

- Create a `.env` file inside the root of the project containing:

```sh
CONNECTION_STRING="paste_your_connection_string_here"
JWT_ISSUER="https://alpimi.pl/"
JWT_EXPIRE=60
HASH_ITERATIONS=10000
HASH_ALGORITHM="SHA256"
TEST_CONNECTION_STRING="paste_your_test_connection_string_here"
AGGRESSIVE_PERMIT_LIMIT=10
AGGRESSIVE_TIME_WINDOW=2
STRICT_PERMIT_LIMIT=10
STRICT_TIME_WINDOW=2
REGULAR_PERMIT_LIMIT=10
REGULAR_TIME_WINDOW=2
MODERATE_PERMIT_LIMIT=10
MODERATE_TIME_WINDOW=2
LOOSE_PERMIT_LIMIT=10
LOOSE_TIME_WINDOW=2
HISTORY_LIMIT=100
JWT_KEY="ee3f34fbce163926832799463f316ba996fcbd7481f44aefb7eb48a4bd9e7f16"
KEY_SIZE=256
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

- Paste your new connection string in a new `.runsettings` file like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
    <RunConfiguration>
        <EnvironmentVariables>
            <TEST_CONNECTION_STRING>paste_your_connection_string_here</TEST_CONNECTION_STRING>

            <AGGRESSIVE_PERMIT_LIMIT>30</AGGRESSIVE_PERMIT_LIMIT>
            <AGGRESSIVE_TIME_WINDOW>5</AGGRESSIVE_TIME_WINDOW>

            <STRICT_PERMIT_LIMIT>30</STRICT_PERMIT_LIMIT>
            <STRICT_TIME_WINDOW>5</STRICT_TIME_WINDOW>

            <REGULAR_PERMIT_LIMIT>30</REGULAR_PERMIT_LIMIT>
            <REGULAR_TIME_WINDOW>5</REGULAR_TIME_WINDOW>

            <MODERATE_PERMIT_LIMIT>30</MODERATE_PERMIT_LIMIT>
            <MODERATE_TIME_WINDOW>5</MODERATE_TIME_WINDOW>

            <LOOSE_PERMIT_LIMIT>30</LOOSE_PERMIT_LIMIT>
            <LOOSE_TIME_WINDOW>5</LOOSE_TIME_WINDOW>
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
