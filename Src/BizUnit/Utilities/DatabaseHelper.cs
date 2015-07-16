//---------------------------------------------------------------------
// File: DatabaseHelper.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2004-2010, Kevin B. Smith. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

namespace BizUnit
{
	using System;
	using System.Data;
    using System.Data.Odbc;
	using System.Data.SqlClient;

	/// <summary>
	/// Static Helper for executing SQL statements
	/// </summary>
	public class DatabaseHelper
	{
        #region constructor(s)

        /// <summary>
        /// Constructor for class, default constructor is private to prevent instances being
        /// created as the class only has static methods
        /// </summary>
        private DatabaseHelper()
		{
        }

        #endregion // constructor(s)

        #region Static Methods
        /// <summary>
        /// Excecutes the SQL statement against the database and returns a DataSet with the results
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlCommand">SQL statement to execute</param>
        /// <returns>DataSet with the results of the executed command</returns>
        public static DataSet ExecuteSqlCommand( string connectionString, string sqlCommand )
        {
            DataSet ds = new DataSet();

            using ( SqlConnection connection = new SqlConnection( connectionString ) )
            {
                SqlDataAdapter adapter = new SqlDataAdapter( sqlCommand, connection );
                adapter.Fill( ds );
            }   // connection

            return ds;
        }

        /// <summary>
        /// Executes the SQL statement and returns the first column of the first row in the resultset returned by the query.
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlCommand">SQL statement to execute</param>
        /// <returns>The contents of the first column of the first row in the resultset</returns>
        public static int ExecuteScalar( string connectionString, string sqlCommand )
        {
            SqlConnection connection = null;
            object col;

            try 
            {
                connection = new SqlConnection( connectionString );
                SqlCommand command = new SqlCommand( sqlCommand, connection );
                command.Connection.Open();
                col = command.ExecuteScalar();
            }
            finally 
            {
                if (null != connection)
                {
                    connection.Close();
                }
            }

            return Convert.ToInt32( col );
        }

        /// <summary>
        /// Executes the SQL statement
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlCommand">SQL statement to execute</param>
        public static int ExecuteNonQuery( string connectionString, string sqlCommand )
        {
            SqlConnection connection = null;
            int numberOfRowsAffected;

            try 
            {
                connection = new SqlConnection( connectionString );
                SqlCommand command = new SqlCommand( sqlCommand, connection );
                command.Connection.Open();
                numberOfRowsAffected = command.ExecuteNonQuery();
            }
            finally 
            {
                if (null != connection)
                {
                    connection.Close();
                }
            }

            return numberOfRowsAffected;
        }
        #endregion // Static Methods

        /// Executes the SQL statement using ODBC
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlQuery">SQL statement to execute</param>
        internal static int ExecuteODBCNonQuery(string connectionString, string sqlQuery)
        {
            int numberOfRowsAffected;

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();
                OdbcCommand command = new OdbcCommand(sqlQuery, connection);
                numberOfRowsAffected = command.ExecuteNonQuery();
            }

            return numberOfRowsAffected;
        }



        /// Executes the SQL statement and returns a single int
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlQuery">SQL statement to execute</param>
        internal static int ExecuteScalarODBCQuery(string connectionString, string sqlQuery)
        {
            int numberOfRowsAffected;

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                connection.Open();
                OdbcCommand command = new OdbcCommand(sqlQuery, connection);
                numberOfRowsAffected = int.Parse(command.ExecuteScalar().ToString()); 
            }

            return numberOfRowsAffected;
        }
    }
}
