## Alpimi planner backend
A WEP API cred in .NET for the Alpimi planner

# Setup for development
gfhjfgh

- Install
```

# Tips

- Install [CSharpier](https://marketplace.visualstudio.com/items?itemName=csharpier.CSharpier), go to `Tools > Options > CSharpier > General` and change `Reformat with CSharpier on Save` to `True`. It will save you a lot of time

- Remember to add a migration whenever an entity is changed:
```
dotnet ef migrations add MigrationName
dotnet ef database update
```
