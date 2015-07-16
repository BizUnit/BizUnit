
namespace BizUnit.TestSteps.Sql
{
    using System;
    using System.Collections.ObjectModel;
    using BizUnitOM;

    ///<summary>
    /// Database query definition
    ///</summary>
    public class SqlQuery
    {
        ///<summary>
        /// The raw Sql query to be executed, can be include formatting instructions which are replaced with the cref="QueryParameters". e.g. select * from dbo.MtTable where runTime > '{0}' AND SystemState = '{1}'
        ///</summary>
        public string RawSqlQuery { get; set; }

        ///<summary>
        /// The parameters to substitute into the the cref="RawSqlQuery"
        ///</summary>
        public Collection<object> QueryParameters { get; set; }

        ///<summary>
        /// Default constructor
        ///</summary>
        public SqlQuery() 
        {
            QueryParameters = new Collection<object>();
        }

        ///<summary>
        /// Formats the query string, replacing the formatting instructions in cref="RawSqlQuery" with the parameters in cref="QueryParameters"
        ///</summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        ///<returns></returns>
        public string GetFormattedSqlQuery(Context context)
        {
            if (QueryParameters.Count > 0)
            {
                var objParams = new object[QueryParameters.Count];
                int c = 0;

                foreach (var obj in QueryParameters)
                {
                    object objValue = obj.GetType() == typeof(ContextProperty) ? ((ContextProperty)obj).GetPropertyValue(context) : obj;

                    if (objValue.GetType() == typeof(DateTime))
                    {
                        // Convert to SQL Datetime
                        objParams[c++] = ((DateTime)objValue).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    else
                    {
                        objParams[c++] = objValue;
                    }
                }

                return string.Format(RawSqlQuery, objParams);
            }
            
            return RawSqlQuery;
        }

        ///<summary>
        /// Validates the SqlQuery
        ///</summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        ///<exception cref="ArgumentNullException"></exception>
        public void Validate(Context context)
        {
            string sqlQuery = GetFormattedSqlQuery(context);
            if (string.IsNullOrEmpty(sqlQuery))
            {
                throw new ArgumentNullException("The Sql Query cannot be formmatted correctly");
            }

            for (int c = 0; c < QueryParameters.Count; c++)
            {
                if (QueryParameters[c] is string)
                {
                    QueryParameters[c] = context.SubstituteWildCards((string)QueryParameters[c]);
                }
            }

            RawSqlQuery = context.SubstituteWildCards(RawSqlQuery);
        }
    }
}
