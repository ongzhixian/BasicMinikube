#Server=(localdb)\MSSQLLocalDB;Integrated Security=true
#Server=(LocalDB)\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=D:\Data\MyDB1.mdf.
#Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EIMA;Integrated Security=True;Encrypt=False;Application Name=aass;Workstation ID=aa
#"Server=(localdb)\mssqllocaldb;Database=Blogging;Trusted_Connection=True;
# -c BlogContext 
# --context-namespace New.Namespace 
#  -t Blog -t Post 

# Required Nuget packages for EF to work (for Sql Server)
# dotnet add .\VirtualFileWebApi\ package Microsoft.EntityFrameworkCore.Design 
# dotnet add .\VirtualFileWebApi\ package Microsoft.EntityFrameworkCore.SqlServer

# Use this when outside project folder
#dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EIMA;Integrated Security=True;Encrypt=False;Application Name=DotnetEf;Workstation ID=SARA" Microsoft.EntityFrameworkCore.SqlServer --project .\VirtualFileWebApi\ --context-dir DbContexts --output-dir DbModels --table virtual_file --no-onconfiguring --force

dotnet ef dbcontext scaffold "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EIMA;Integrated Security=True;Encrypt=False;Application Name=DotnetEf;Workstation ID=SARA" Microsoft.EntityFrameworkCore.SqlServer --project .\VirtualFileWebApi.csproj --context-dir DbContexts --output-dir DbModels --table virtual_file --no-onconfiguring --force
