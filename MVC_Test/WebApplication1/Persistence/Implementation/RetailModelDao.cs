using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using WebApplication1.Persistence.DBConnectionFactory;
using WebApplication1.Persistence.Interface;

namespace WebApplication1.Persistence.Implementation
{
    public class RetailModelDao : IRetailModelDao
    {

        #region Private variables and constructors

        protected readonly IDbConnectionFactory factory;

        public RetailModelDao()
        {
            this.factory = new DbConnectionFactory("DefaultDb");
        }

        #endregion

        public List<RetailModel> GetRetailByYear(Int32 Year)
        {
            List<RetailModel> retailList = null;

            DynamicParameters param = new DynamicParameters();
            if (Year > 0)
            {
                param.Add("@Year", Year, dbType: DbType.Int32);
            }
            using (IDbConnection conn = factory.GetConnection())
            {
                conn.Open();
                string SQL = @"[usp_GetRetailByYear]";
                retailList = conn.Query<RetailModel>(SQL, param, commandType: CommandType.StoredProcedure).ToList();
                conn.Close();
            }
            return retailList;
        }

        public RetailModel GetRetailByUniqueId(Guid UniqueId)
        {
            RetailModel retail = null;

            DynamicParameters param = new DynamicParameters();
            if (UniqueId!=Guid.Empty)
            {
                param.Add("@UniqueId", UniqueId.ToString(), dbType: DbType.String);
            }
            using (IDbConnection conn = factory.GetConnection())
            {
                conn.Open();
                string SQL = @"SELECT 
Id
,UniqueId
,ECS
,BCat
,Series
,Jan
,Feb
,Mar
,Apr
,May
,Jun
,Jul
,Aug
,Sep
,Oct
,Nov
,Dec
,CalendarYear
,(ISNULL(Jan,0)+ISNULL(Feb,0)+ISNULL(Mar,0)+ISNULL(Apr,0)+ISNULL(May,0)+ISNULL(Jun,0)+ISNULL(Jul,0)+ISNULL(Aug,0)+ISNULL(Sep,0)+ISNULL(Oct,0)+ISNULL(Nov,0)+ISNULL(Dec,0)) AS Total

FROM Retails WHERE UniqueId=@UniqueId AND ISNULL(IsDeleted,0)=0";
                retail = conn.Query<RetailModel>(SQL, param)?.FirstOrDefault();
                conn.Close();
            }
            return retail;
        }

        public int SaveRetail(RetailModel P)
        {
            int result = 0;
            DynamicParameters param = new DynamicParameters();

            param.Add("@SystemIp", P.SystemIp, DbType.String);
          
           
            param.Add("@UniqueId", P.UniqueId, DbType.Guid);

            param.Add("@Jul", P.Jul, DbType.Int32);
            param.Add("@Aug", P.Aug, DbType.Int32);
            param.Add("@Sep", P.Sep, DbType.Int32);
            param.Add("@Oct", P.Oct, DbType.Int32);
            param.Add("@Nov", P.Nov, DbType.Int32);
            param.Add("@Dec", P.Dec, DbType.Int32);

            using (IDbConnection conn = factory.GetConnection())
            {
                conn.Open();
                string SQL = @"[usp_Retail_Save]";
                result = conn.Execute(SQL, param, commandType: CommandType.StoredProcedure);
                conn.Close();
            }
            return result;
        }

        public int DeleteRetail(RetailModel P)
        {
            int result = 0;
            DynamicParameters param = new DynamicParameters();
            param.Add("@UniqueId", P.UniqueId, DbType.Guid);
            param.Add("@SystemIp", P.SystemIp, DbType.String);

            using (IDbConnection conn = factory.GetConnection())
            {
                conn.Open();
                string SQL = @"[usp_Retail_Delete]";
                result = conn.Execute(SQL, param, commandType: CommandType.StoredProcedure);
                conn.Close();
            }
            return result;
        }

        public int SaveDisplayRetailLog(Int32 Year)
        {
            int result = 0;
            DynamicParameters param = new DynamicParameters();
            if (Year > 0)
            {
                param.Add("@Year", Year, dbType: DbType.Int32);
            }

            using (IDbConnection conn = factory.GetConnection())
            {
                conn.Open();
                string SQL = @"[usp_Save_Display_Retail_Logs]";
                result = conn.Execute(SQL, param, commandType: CommandType.StoredProcedure);
                conn.Close();
            }
            return result;
        }
    }
}