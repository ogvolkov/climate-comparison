﻿using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ClimateComparison.DataAccess.Infra
{
    public class SqlConnectionProvider
    {
        private readonly IConfiguration _configuration;

        public SqlConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public SqlConnection Get()
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string is not present");
            }

            return new SqlConnection(connectionString);
        }
    }
}