# WareLogix.Configuration

Standard package for setting configuration requirements for WareLogix projects.

## Version History

### 1.0.6 

Changes:

Update .csproj file to include DV2001 to list of warnings to suppress for compiler.
If we do not want to update .csproj file, we have to add a `-nowarn:DV2001` to dotnet build command

```ps1 ;Example of using -nowarn flag
dotnet build -nowarn:DV2001
```

```txt ;Example of DV2001 warning
CSC : warning DV2001: The project does not reference any Dependency Validation diagrams or referenced diagrams are not valid. 
Dependency validation will not be performed. (https://go.microsoft.com/fwlink/?linkid=2110178) 
[C:\src\github.com\ongzhixian\Minikube\WareLogix.Configuration\WareLogix.Configuration.csproj]
```

We see this because we have Dependency Validation tools installed in Visual Studio.
Oddly, it affects `dotnet build` as well. 
Not sure how that happens.
But in any case, since we are not using the Visual Studio architectural tools 
(which is use to define the Dependency Validation), we can disable this warning.