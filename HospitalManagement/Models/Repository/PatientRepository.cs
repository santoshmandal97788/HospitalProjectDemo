using Dapper;
using HospitalManagement.Models.IRepository;
using HospitalManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace HospitalManagement.Models.Repository
{

    public class PatientRepository : IPatientRepository
    {

        private IDbConnection _db;
        public PatientRepository()
        {
            _db = new SqlConnection(@"Data Source=DESKTOP-BRE9TRN\SQLEXPRESS; Integrated security=true; Initial Catalog=HospitalDB;");
        }
      

        public bool DeletePatient(int Id)
        {
            //int deletedPatient = this._db.Execute("spDeletePatient", new { patid = Id }, commandType: CommandType.StoredProcedure);
            //return deletedPatient > 0;

            SqlParameter[] parameters =
                            {
                            new SqlParameter("@id",Id)
                          };

            string query = "spDeletePatient";
            var args = new DynamicParameters(new { });

            parameters.ToList().ForEach(p => args.Add(p.ParameterName, p.Value));
            try
            {
                //this._db.Query<UserDetails>(query, args).SingleOrDefault();
                this._db.Execute(query,args, commandType: CommandType.StoredProcedure);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public PatientViewModel FindById(int Id)
        {
             PatientViewModel pvm = _db.QuerySingleOrDefault<PatientViewModel>("spGetPatientById", new { Id = Id }, commandType: CommandType.StoredProcedure);
            return pvm;
           

        }

        public List<PatientViewModel> GetAllPatient()
        {
            return _db.Query<PatientViewModel>("spGetAllPatient", null, commandType: CommandType.StoredProcedure).ToList();
        }

       

        public bool UpdatePatient(PatientViewModel patient)
        {
            SqlParameter[] parameters =
                                       {
                                            new SqlParameter("@patImage",patient.PatImage),
                                            new SqlParameter("@patName",patient.PatName),
                                            new SqlParameter("@patContact",patient.PatContact),
                                            new SqlParameter("@isActive",patient.IsActive),
                                            new SqlParameter("@entryDate",System.DateTime.Now),
                                            new SqlParameter("@id",patient.Id)
                                        };
            string query = "spUpdatePatient";

            var args = new DynamicParameters(new { });
            parameters.ToList().ForEach(p => args.Add(p.ParameterName, p.Value));
            try
            {
                this._db.Execute(query, args, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }


        public bool AddPatient(PatientViewModel patient)
        {
            SqlParameter[] parameters =
                             {
                            new SqlParameter("@patImage",patient.PatImage),
                            new SqlParameter("@patName",patient.PatName),
                            new SqlParameter("@patContact",patient.PatContact),
                            new SqlParameter("@isActive",patient.IsActive),
                            new SqlParameter("@entryDate",System.DateTime.Now),
                          };

            string query = "spCreatePatient";
            var args = new DynamicParameters(new { });

            parameters.ToList().ForEach(p => args.Add(p.ParameterName, p.Value));
            try
            {
                //this._db.Query<UserDetails>(query, args).SingleOrDefault();
                this._db.Execute(query, args, commandType: CommandType.StoredProcedure);
            }
            catch
            {
                return false;
            }

            return true;
        }

       
    }
    
}