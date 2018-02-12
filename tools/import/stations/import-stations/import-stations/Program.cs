using System;
using System.Data.SqlClient;
using System.IO;

namespace import_stations
{
    class Program
    {
        static void Main(string[] args)
        {  
            string fileName = args[0];

            using (var sqlConnection = new SqlConnection(args[1]))
            {
                sqlConnection.Open();

                using (var sqlCommand = sqlConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "INSERT INTO Stations(Id, Name, Location, Country)" +
                                             " VALUES(@Id, @Name, @Location, @Country)";

                    using (var streamReader = new StreamReader(fileName))
                    {
                        for (;;)
                        {
                            string line = streamReader.ReadLine();
                            if (line == null)
                            {
                                break;
                            }

                            if (line[0] == '%')
                            {
                                continue;
                            }

                            string[] fields = line.Split('\t');

                            int id = int.Parse(fields[0]);
                            string name = fields[1].Trim();
                            float latitude = float.Parse(fields[2]);
                            float longitude = float.Parse(fields[3]);
                            string country = fields[8].Trim();

                            sqlCommand.Parameters.Clear();
                            sqlCommand.Parameters.AddWithValue("@Id", id);
                            sqlCommand.Parameters.AddWithValue("@Name", name);
                            sqlCommand.Parameters.AddWithValue("@Location", $"POINT({longitude} {latitude})");
                            sqlCommand.Parameters.AddWithValue("@Country", country);
                            sqlCommand.ExecuteNonQuery();

                            Console.WriteLine($"{name} imported");
                        }
                    }
                }
            }
        }
    }
}