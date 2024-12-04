Console.WriteLine("PRESS [ENTER] TO START");
Console.ReadLine();

ReadDB();

void ReadDB()
{
    string connectionString = "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;Initial Catalog=EIMA";
    using Microsoft.Data.SqlClient.SqlConnection sqlConnection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);

    var cmd = sqlConnection.CreateCommand();
    cmd.CommandText = "SELECT id, name, is_active FROM student WHERE id = 1;";

    sqlConnection.Open();
    var reader = cmd.ExecuteReader();
    if (reader.Read())
        Console.WriteLine($"Student ID = {reader["id"]}, Name = {reader["name"]}, is_active = {reader["is_active"]}");
}