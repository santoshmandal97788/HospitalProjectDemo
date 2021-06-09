using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HospitalManagement.Models.ViewModel
{
    public class PatientViewModel
    {
        public int Id { get; set; }
        public byte[] PatImage { get; set; }
        public string PatName { get; set; }
        public string PatContact { get; set; }
        public bool IsActive { get; set; }
        public DateTime EntryDate { get; set; }
    }
}