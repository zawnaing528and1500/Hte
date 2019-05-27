using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using EventTicket.App_Code;
using System.Data;
using System.Text;
//using OfficeOpenXml;



namespace EventTicket.Controllers
{
    public class TestController : Controller
    {
        public object IO { get; private set; }
        DBBase db = new DBBase();
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }
        public void ReplaceLastCharacter()
        {
            string str = "ကပ118560";
            int i = 1;
            //Replace 1 at 0
            str = str.Remove(str.Length - 1, 1) + i;
            Debug.WriteLine(str);
        }
        public ActionResult PrintCharacter()
        {
            int unicode = 65;
            char character = (char)unicode;
            string text = character.ToString();
            Debug.WriteLine(text);
            return View();
        }
        public ActionResult SendSeatInfo()
        {
            MyWebRequest myRequest = new MyWebRequest("http://smsgateway.me/api/v3/messages/send", "POST", "email=inspireeventticket@gmail.com&password=companyproject&device=83001&number=+959257706564&message=Ko Sai! I love you very much!");
            //show the response string on the console screen.
            ViewData["status"] = myRequest.GetResponse();
            return View();
        }

        public void DeleteFileFromFolder(string StrFilename)
        {

            string strPhysicalFolder = Server.MapPath("~/Image/Cover/");

            string strFileFullPath = strPhysicalFolder + StrFilename;

            if (System.IO.File.Exists(strFileFullPath))
            {
                System.IO.File.Delete(strFileFullPath);
            }
        }

        public ActionResult DeleteImage()
        {
            DeleteFileFromFolder("IPMS Coventry  Warwickshire Show 2018.jpg");
            return View();
        }
        public ActionResult ProcessDataToExcel()
        {
            DataToExcel();
            return View();
        }
        private void DataToExcel()
        {
            DataTable dt = db.getAllByQuery("select ID,No,Date from HteNumber");//your datatable
            string attachment = "attachment; filename=download.xls";

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();

            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            Response.ContentEncoding = System.Text.Encoding.UTF8;

            string tab = "";
            foreach (DataColumn dc in dt.Columns)
            {
                Response.Write(tab + dc.ColumnName);
                tab = "\t";
            }
            Response.Write("\r\n");
            int i;
            foreach (DataRow dr in dt.Rows)
            {
                tab = "";
                for (i = 0; i < dt.Columns.Count; i++)
                {
                    Response.Write(tab +  dr[i].ToString());
                    tab = "\t";
                }
                Response.Write("\r\n");
            }

            Response.Flush();
            Response.End();
        }
        public ActionResult ExcelUploadForm()
        {
            return View();
        }
        //public ActionResult ProcessExcelUploadForm(FormCollection formCollection)
        //{
        //    string[] HteNumber = new string[] { };
        //    if (Request != null)
        //    {
        //        HttpPostedFileBase file = Request.Files["UploadedFile"];
        //        if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
        //        {
        //            string fileName = file.FileName;
        //            string fileContentType = file.ContentType;
        //            byte[] fileBytes = new byte[file.ContentLength];
        //            var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
        //            using (var package = new ExcelPackage(file.InputStream))
        //            {
        //                var currentSheet = package.Workbook.Worksheets;
        //                var workSheet = currentSheet.First();
        //                var noOfCol = workSheet.Dimension.End.Column;
        //                var noOfRow = workSheet.Dimension.End.Row;

        //                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
        //                {
        //                    HteNumber = HteNumber.Concat(new string[] { workSheet.Cells[rowIterator, 1].Value.ToString()}).ToArray();
        //                    HteNumber = HteNumber.Concat(new string[] { workSheet.Cells[rowIterator, 1].Value.ToString(workSheet.Cells[rowIterator, 2].Value.ToString()) }).ToArray();
        //                }
        //            }
        //        }
        //    }
        //    return View();
        //}

    }
}