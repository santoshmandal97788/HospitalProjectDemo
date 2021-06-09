using Dapper;
using HospitalManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HospitalManagement.Models
{
    public static class Helper
    {
        private static IDbConnection _db = new SqlConnection(@"Data Source=DESKTOP-BRE9TRN\SQLEXPRESS; Integrated security=true; Initial Catalog=HospitalDB;");


        //public Helper()
        //{
        //    _db = new SqlConnection(@"Data Source=DESKTOP-BRE9TRN\SQLEXPRESS; Integrated security=true; Initial Catalog=HospitalDB;");
        //}
        public static int TotalCount()
        {

            var returnValue = _db.ExecuteScalar("spTotalPatientCount", commandType: CommandType.StoredProcedure);

            return (int)returnValue;
        }

        public static int ActiveTotalCount()
        {
            var returnValue = _db.ExecuteScalar("spActivePatientCount", commandType: CommandType.StoredProcedure);

            return (int)returnValue;

        }
        public static int InactiveTotalCount()
        {
            var returnValue = _db.ExecuteScalar("spInActivePatientCount", commandType: CommandType.StoredProcedure);

            return (int)returnValue;

        }
        public static int TodayAdmitCount()
        {
            var returnValue = _db.ExecuteScalar("spTodayAdmitCount", commandType: CommandType.StoredProcedure);

            return (int)returnValue;

        }
        //public static int GetTotalUser()
        //{
        //    return _db.UserRoles.Where(b => b.RoleId == 2).Count();
        //}
        //public static int GetTotalNewOrder()
        //{
        //    return _db.tblOrders.Where(b => b.DeliveredStatus == "Pending").Count();
        //}
        //public static int GetTotalBooking()
        //{
        //    return _db.tblBookings.Count();
        //}
    }
}