using System;
using System.Data.SqlClient;
using System.IO;

namespace import_cities
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
                sqlCommand.CommandText = "INSERT INTO Cities(Name, AltNames, Location, CountryCode, Population)" +
                    " VALUES(@Name, @AltNames, @Location, @CountryCode, @Population)";

                    using (var streamReader = new StreamReader(fileName))
                    {
                        for (;;)
                        {
                            string line = streamReader.ReadLine();
                            if (line == null)
                            {
                                break;
                            }

                            string[] fields = line.Split('\t');

                            string name = fields[1];
                            string altNames = fields[3];
                            float latitude = float.Parse(fields[4]);
                            float longitude = float.Parse(fields[5]);
                            string countryCode = fields[8];
                            int population = int.Parse(fields[14]);

                            sqlCommand.Parameters.Clear();
                            sqlCommand.Parameters.AddWithValue("@Name", name);
                            sqlCommand.Parameters.AddWithValue("@AltNames", altNames);
                            sqlCommand.Parameters.AddWithValue("@Location", $"POINT({longitude} {latitude})");
                            sqlCommand.Parameters.AddWithValue("@CountryCode", countryCode);
                            sqlCommand.Parameters.AddWithValue("@Population", population);
                            sqlCommand.ExecuteNonQuery();
                            
                            Console.WriteLine($"{name} imported");
                        }
                    }
                }
            }
        }
    }
}
