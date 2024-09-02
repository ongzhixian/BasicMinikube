
using Microsoft.Extensions.Configuration;

//
// %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json

//Microsoft.Extensions.Configuration.UserSecretsConfigurationExtensions.AddUserSecrets()

IConfiguration Configuration = new ConfigurationBuilder()
   .AddUserSecrets("47df7034-c1c1-4b87-8373-89f5e42fc9ec")
   .Build();


Console.WriteLine("All done");