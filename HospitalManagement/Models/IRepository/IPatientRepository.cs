using HospitalManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalManagement.Models.IRepository
{
    public interface IPatientRepository
    {

        List<PatientViewModel> GetAllPatient();
        PatientViewModel FindById(int Id);
        bool AddPatient(PatientViewModel patient);
        bool UpdatePatient(PatientViewModel patient);
        bool DeletePatient(int Id);
    }
}
