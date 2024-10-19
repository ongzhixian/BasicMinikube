string connectionString = "database=mini;server=localhost;port=23306;user id=root;password=Pass1234";

MySql.Data.MySqlClient.MySqlConnection conn;

try
{
    MySql.Data.MySqlClient.MySqlConnectionStringBuilder connStringBuilder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();
    connStringBuilder.Database = "mini";
    connStringBuilder.Server = "localhost";
    connStringBuilder.Port = 23306;
    connStringBuilder.UserID = "root";
    connStringBuilder.Password = "Pass1234";
    // 

    //Console.WriteLine($"Attempting to connect to mysql: {connStringBuilder.ToString()}");
    conn = new MySql.Data.MySqlClient.MySqlConnection(connStringBuilder.ToString());
    conn.Open();
    Console.WriteLine("MySql connection opened");
    conn.Close();
}
catch (MySql.Data.MySqlClient.MySqlException ex)
{
    Console.WriteLine(ex.Message);
}
