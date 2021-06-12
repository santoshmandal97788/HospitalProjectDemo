using HospitalManagement.Models.Repository;

using HospitalManagement.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HospitalManagement.Controllers
{
    public class PatientController : Controller
    {
       
        PatientRepository pr = new PatientRepository();
        // GET: Patient

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetList()
        {
            var patList = pr.GetAllPatient().Select(x => new { Id = x.Id, PatName = x.PatName, PatContact = x.PatContact, IsActive = x.IsActive, EntryDate= x.EntryDate.ToShortDateString(), PatImage= Convert.ToBase64String(x.PatImage) }).ToList();

            return Json(new { data = patList }, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult Edit(int id)
        {

            return View(pr.FindById(id));
        }
        [HttpPost]
        public ActionResult Edit([Bind(Exclude = "PatImage")] PatientViewModel pvm)
        {
            var patImage = Request.Files["PatImage"];
            var patient = pr.FindById(pvm.Id);
            if (Session["imageData"] == null)
            {
                if (patImage.ContentLength ==0 && patImage.FileName=="" && patImage.ContentType=="application/octet-stream")
                {
                    pvm.PatImage = patient.PatImage;
                    pr.UpdatePatient(pvm);
                }
                else
                {
                    byte[] imageData = ConvertImage();
                    pvm.PatImage = imageData;
                    pr.UpdatePatient(pvm);
                }
               
            }
            else
            {
                byte[] imageData = ConvertImage();
                pvm.PatImage = imageData;
                pr.UpdatePatient(pvm);
                Session["imageData"] = null;
            }
            return RedirectToAction("Index", "Patient");

        }
       
        [HttpPost]
        public ActionResult Delete(int id)
        {
                pr.DeletePatient(id);
            
                return Json(new { success = true, message = "Record Deleted Successfully" }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Details(int id)
        {
           PatientViewModel patient= pr.FindById(id);
           
            return View(patient);
        }
        public ActionResult Print(int id)
        {

            PatientViewModel patient = pr.FindById(id);

            return View(patient);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create([Bind(Exclude = "PatImage")] PatientViewModel pvm)
        {
            //byte[] imageData = ConvertImage();
            //pvm.PatImage = imageData;

            //pr.AddPatient(pvm);
            //return RedirectToAction("Index", "Patient");

            if (Session["imageData"] == null)
            {
                byte[] imageData = ConvertImage();
                pvm.PatImage = imageData;
                pr.AddPatient(pvm);

            }
            else
            {
                //byte[] data = (byte[])System.Web.HttpContext.Current.Session["imageData"];
                //pvm.PatImage = data;
                //pr.AddPatient(pvm);
                byte[] imageData = ConvertImage();
                pvm.PatImage = imageData;
                pr.AddPatient(pvm);
                Session["imageData"] = null;
            }
            return RedirectToAction("Index", "Patient");
        }



        private byte[] ConvertImage()
        {
            byte[] imageData = null;
            if (Request.Files.Count > 0 && Session["imageData"] == null)
            {
                HttpPostedFileBase pf = Request.Files["PatImage"];
                System.Drawing.Image bm = System.Drawing.Image.FromStream(pf.InputStream);
                bm = ResizeImage((Bitmap)bm, 98, 118); /// new width, heig

                imageData = imageToByteArray(bm);
                //using (var binaryReader = new BinaryReader(objFiles.InputStream))
                //{
                //    imageData = binaryReader.ReadBytes(objFiles.ContentLength);

                //}

            }
            if (Session["imageData"] != null)
            {
                //HttpPostedFileBase pf = Request.Files["PatImage"];
                //System.Drawing.Image bm = System.Drawing.Image.FromStream(pf.InputStream);
                System.Drawing.Image bm = (Image)Session["imageData"];
                bm = ResizeImage((Bitmap)bm, 98, 118); /// new width, heig

                imageData = imageToByteArray(bm);
                //using (var binaryReader = new BinaryReader(objFiles.InputStream))
                //{
                //    imageData = binaryReader.ReadBytes(objFiles.ContentLength);

                //}

            }
            return imageData;
        }
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    //if (image.Width>2423)
                    //{
                    //    image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    //}
                   
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            

            return destImage;
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return ms.ToArray();
        }


        [HttpPost]
        public void Capture(string imageData)
        {
            string capturedImage = imageData;
            byte[] data = Convert.FromBase64String(capturedImage);

            Image img = byteArrayToImage(data);
            Session["imageData"] = img;

        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
        [HttpGet]
        public ActionResult PieChart()
        {
            List<PieChartViewModel> pieChartResult = new List<PieChartViewModel>();
            var activepatient = pr.GetAllPatient().Where(a => a.IsActive == true).Count();
            var dischargedPatient = pr.GetAllPatient().Where(a => a.IsActive == false).Count();
            pieChartResult.Add(new PieChartViewModel()
            {
                Name = "Admitted",
                TotalRecord = activepatient
            });
            pieChartResult.Add(new PieChartViewModel()
            {
                Name = "Discharged",
                TotalRecord = dischargedPatient
            });
            return Json(pieChartResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult barChart()
        {
            List<BarChartViewModel> barChartResult = new List<BarChartViewModel>();
            var activepatient = pr.GetAllPatient().Where(a => a.EntryDate.ToShortDateString() == DateTime.Now.ToShortDateString() && a.IsActive == true).Count();
            var dischargedPatient = pr.GetAllPatient().Where(a => a.EntryDate.ToShortDateString() == DateTime.Now.ToShortDateString() && a.IsActive == false).Count();
            barChartResult.Add(new BarChartViewModel()
            {
                Name = "Admitted",
                TotalRecord = activepatient
            });
            barChartResult.Add(new BarChartViewModel()
            {
                Name = "Discharged",
                TotalRecord = dischargedPatient
            });
            return Json(barChartResult, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetPatientByDate(string Date)
        {
            int value = Convert.ToInt32(Date);
            List<BarChartViewModel> ptdResult = new List<BarChartViewModel>();
            switch (value)
            {
                case 1:
                    var activepatient1 = pr.GetAllPatient().Where(a => a.EntryDate.ToShortDateString() == DateTime.Now.ToShortDateString() && a.IsActive == true).Count();
                    var dischargedPatient1 = pr.GetAllPatient().Where(a => a.EntryDate.ToShortDateString() == DateTime.Now.ToShortDateString() && a.IsActive == false).Count();
                    ptdResult.Add(new BarChartViewModel()
                    {
                        Name = "Admitted",
                        TotalRecord = activepatient1
                    });
                    ptdResult.Add(new BarChartViewModel()
                    {
                        Name = "Discharged",
                        TotalRecord = dischargedPatient1
                    });
                    break;
                case 2:
                    var activepatient2 = pr.GetAllPatient().Where(a => a.EntryDate == DateTime.Today.AddDays(-1) && a.IsActive == true).Count();
                    var dischargedPatient2 = pr.GetAllPatient().Where(a => a.EntryDate == DateTime.Today.AddDays(-1) && a.IsActive == false).Count();
                    ptdResult.Add(new BarChartViewModel()
                    {
                        Name = "Admitted",
                        TotalRecord = activepatient2
                    });
                    ptdResult.Add(new BarChartViewModel()
                    {
                        Name = "Discharged",
                        TotalRecord = dischargedPatient2
                    });
                    break;

                case 3:
                    var activepatient3 = pr.GetAllPatient().Where(a => a.EntryDate == DateTime.Today.AddDays(-2) && a.IsActive == true).Count();
                    var dischargedPatient3 = pr.GetAllPatient().Where(a => a.EntryDate == DateTime.Today.AddDays(-2) && a.IsActive == false).Count();
                    ptdResult.Add(new BarChartViewModel()
                    {
                        Name = "Admitted",
                        TotalRecord = activepatient3
                    });
                    ptdResult.Add(new BarChartViewModel()
                    {
                        Name = "Discharged",
                        TotalRecord = dischargedPatient3
                    });
                    break;
                case 4:
                    var activepatient4 = pr.GetAllPatient().Where(a => a.EntryDate == DateTime.Now.AddDays(-7) && a.IsActive == true).Count();
                    var dischargedPatient4 = pr.GetAllPatient().Where(a => a.EntryDate == DateTime.Now.AddDays(-7) && a.IsActive == false).Count();
                    ptdResult.Add(new BarChartViewModel()
                    {
                        Name = "Admitted",
                        TotalRecord = activepatient4
                    });
                    ptdResult.Add(new BarChartViewModel()
                    {
                        Name = "Discharged",
                        TotalRecord = dischargedPatient4
                    });
                    break;
                default:
                    break;
            }
            return Json(ptdResult, JsonRequestBehavior.AllowGet);

        }
    }
}