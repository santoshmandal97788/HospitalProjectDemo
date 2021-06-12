using HospitalManagement.Models.Repository;
using HospitalManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace HospitalManagement.Controllers
{
    public class PatientAPIController : ApiController
    {
        PatientRepository pr = new PatientRepository();
        // GET: api/PatientAPI
        public IEnumerable<PatientViewModel> Get()
        {
            return pr.GetAllPatient();
        }

        // GET: api/PatientAPI/5
        public PatientViewModel Get(int id)
        {
            return pr.FindById(id);
        }

        // POST: api/PatientAPI
        [System.Web.Http.Route("api/PatientAPI/Post")]
        public HttpResponseMessage Post()
        {
            PatientViewModel pvm = new PatientViewModel();            
           
            pvm.PatName = HttpContext.Current.Request.Params["PatName"];
            pvm.PatContact = HttpContext.Current.Request.Params["PatContact"];
            var isActive = HttpContext.Current.Request.Params["IsActive"];
            pvm.IsActive = Convert.ToBoolean(isActive);
            pvm.EntryDate = DateTime.Now;          

            if (HttpContext.Current.Session["imageData"] == null)
            {
                byte[] imageData = ConvertImage();
                pvm.PatImage = imageData;
                pr.AddPatient(pvm);

            }
            else
            {
                
                byte[] imageData = ConvertImage();
                pvm.PatImage = imageData;
                pr.AddPatient(pvm);
                HttpContext.Current.Session["imageData"] = null;
            }
            return Request.CreateResponse(HttpStatusCode.OK);
          
        }

        // PUT: api/PatientAPI/5
        public HttpResponseMessage Put()
        {
            PatientViewModel pvm = new PatientViewModel();
          
            pvm.Id = Convert.ToInt32 (HttpContext.Current.Request.Params["Id"]);
            pvm.PatName = HttpContext.Current.Request.Params["PatName"];
            pvm.PatContact = HttpContext.Current.Request.Params["PatContact"];
            var isActive = HttpContext.Current.Request.Params["IsActive"];
            pvm.IsActive = Convert.ToBoolean(isActive);
            var PatImage = HttpContext.Current.Request.Params["PatImage"];
            //pvm.PatImage = PatImage;
            //var i= PatImage.GetType();
            var patient = pr.FindById(pvm.Id);
            pvm.EntryDate = patient.EntryDate;

            if (HttpContext.Current.Session["imageData"] == null)
            {
                byte[] imageData = ConvertImage();
                pvm.PatImage = imageData;
                pr.UpdatePatient(pvm);

            }
            else
            {
                //byte[] data = (byte[])System.Web.HttpContext.Current.Session["imageData"];
                //pvm.PatImage = data;
                //pr.AddPatient(pvm);
                byte[] imageData = ConvertImage();
                pvm.PatImage = imageData;
                pr.UpdatePatient(pvm);
                HttpContext.Current.Session["imageData"] = null;
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE: api/PatientAPI/5
        public HttpResponseMessage Delete(int id)
        {
            var patient = pr.FindById(id);
            string message = "Record Deleted Successfully";
            string messagenodata = "Data Not found/ Might be Deleted or Removed";
            HttpResponseMessage response;
            if (patient == null)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, messagenodata);
            }
            else
            {
                pr.DeletePatient(id);
                response = Request.CreateResponse(HttpStatusCode.OK, message);
            }
            return response;
        }

        private byte[] ConvertImage()
        {
            byte[] imageData = null;
            if (HttpContext.Current.Request.Files.Count > 0 && HttpContext.Current.Session["imageData"] == null)
            {
                HttpPostedFile pf = HttpContext.Current.Request.Files["PatImage"];
                System.Drawing.Image bm = System.Drawing.Image.FromStream(pf.InputStream);
                bm = ResizeImage((Bitmap)bm, 98, 118); /// new width, heig

                imageData = imageToByteArray(bm);
              

            }
            if (HttpContext.Current.Session["imageData"] != null)
            {
              
                System.Drawing.Image bm = (Image)HttpContext.Current.Session["imageData"];
                bm = ResizeImage((Bitmap)bm, 98, 118); /// new width, heig

                imageData = imageToByteArray(bm);
               
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


        [System.Web.Http.Route("api/PatientAPI/Capture")]
        [System.Web.Http.HttpPost]
        public void Post(string imageData)
        {
            string capturedImage = imageData;
            byte[] data = Convert.FromBase64String(capturedImage);

            Image img = byteArrayToImage(data);
            HttpContext.Current.Session["imageData"] = img;

        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}
