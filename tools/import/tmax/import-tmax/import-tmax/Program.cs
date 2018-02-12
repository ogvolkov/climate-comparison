using System;
using System.Data.SqlClient;
using System.IO;

namespace import_tmax
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
                    sqlCommand.CommandText = "INSERT INTO AverageHigh(StationId, OriginalDate, Year, Month, Temperature)" +
                                             " VALUES(@StationId, @OriginalDate, @Year, @Month, @Temperature)";

                    int linesImported = 0;
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

                            int stationId = int.Parse(fields[0]);
                            float originalDate = float.Parse(fields[2]);
                            int year = (int)Math.Truncate(originalDate);
                            int month = (int)Math.Round((originalDate - year) * 12 + 0.5);
                            float temperature = float.Parse(fields[3]);
                            
                            sqlCommand.Parameters.Clear();
                            sqlCommand.Parameters.AddWithValue("@StationId", stationId);
                            sqlCommand.Parameters.AddWithValue("@originalDate", originalDate);
                            sqlCommand.Parameters.AddWithValue("@year", year);
                            sqlCommand.Parameters.AddWithValue("@month", month);                            
                            sqlCommand.Parameters.AddWithValue("@temperature", temperature);                            
                            sqlCommand.ExecuteNonQuery();

                            if (++linesImported % 10000 == 0)
                            {
                                Console.WriteLine($"line #{linesImported}, {stationId} {originalDate} data imported");
                            }
                        }
                    }
                }
            }
        }
    }
}