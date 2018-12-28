using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace import_precipitation
{
    /// <summary>
    /// Import precipitation data from CRU TS v. 4.02 https://crudata.uea.ac.uk/cru/data/hrg/
    /// </summary>
    class Program
    {
        private const int NO_DATA_VALUE = -999;

        static void Main(string[] args)
        {
            string fileName = args[0];
            int startYear = int.Parse(args[1]);
            int colCount = int.Parse(args[2]);
            int rowCount = int.Parse(args[3]);

            int year = startYear;
            int month = 1;
            int row = 0;

            using (var reader = File.OpenText(fileName))
            {
                using (var sqlConnection = new SqlConnection(args[4]))
                {
                    sqlConnection.Open();
                    using (var sqlCommand = sqlConnection.CreateCommand())
                    {
                        sqlCommand.CommandText = "INSERT INTO Precipitation(Location, Year, Month, Precipitation)" +
                                                 " VALUES(@Location, @Year, @Month, @Precipitation)";

                        for (; ;)
                        {
                            string line = reader.ReadLine();
                            if (line == null)
                            {
                                break;
                            }

                            float latitude = 0.25f + (float)row * 180 / rowCount - 90;

                            int col = 0;
                            foreach (int value in line.Split(' ').Where(it => it != "").Select(int.Parse))
                            {
                                float longitude = 0.25f + (float)col * 360 / colCount - 180;

                                if (value != NO_DATA_VALUE)
                                {
                                    float mm = (float)value / 10;

                                    sqlCommand.Parameters.Clear();
                                    sqlCommand.Parameters.AddWithValue("@Location", $"POINT({longitude} {latitude})");
                                    sqlCommand.Parameters.AddWithValue("@Year", year);
                                    sqlCommand.Parameters.AddWithValue("@Month", month);
                                    sqlCommand.Parameters.AddWithValue("@Precipitation", mm);
                                    sqlCommand.ExecuteNonQuery();
                                }

                                ++col;
                            }

                            ++row;
                            if (row == rowCount)
                            {
                                ++month;
                                if (month > 12)
                                {
                                    ++year;
                                    month = 1;
                                }
                                row = 0;
                            }
                        }
                    }
                }
            }
        }
    }
}
