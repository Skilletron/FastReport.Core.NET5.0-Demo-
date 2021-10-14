using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastReport.Web;
using FastReportWebCore.MVC.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;


namespace FastReportWebCore.MVC.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        IHostingEnvironment _hostingEnvironment;
      

        public HomeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet("{reportIndex:int?}")]
        public IActionResult Index(int? reportIndex = 0)
        {
            var model = new HomeModel()
            {
                WebReport = new WebReport(),
                ReportsList = new[]
                {
                    "Simple List",
                    "Labels",
                    "Master-Detail",
                    "Badges",
                    "Interactive Report, 2-in-1",
                    "Hyperlinks, Bookmarks",
                    "Outline",
                    "Complex (Hyperlinks, Outline, TOC)",
                    "Drill-Down Groups",
                    "Mail Merge",
                    "Polygon",
                    "Chart",
                    "Hello, FastReport!",
                    "Print Entered Value",
                    "Filtering with CheckedListBox",
                    "Filtering with Ranges",
                    "Cascaded Data Filtering",
                    "Handle Dialog Forms",
                    "Dialog Elements"
                },
            };

            var reportToLoad = model.ReportsList[0];
            if (reportIndex >= 0 && reportIndex < model.ReportsList.Length)
                reportToLoad = model.ReportsList[reportIndex.Value];

            string dir = FindReportsFolder(Environment.CurrentDirectory);

            model.WebReport.Report.Load(Path.Combine(dir, $"{reportToLoad}.frx"));

            var dataSet = new DataSet();
            dataSet.ReadXml(Path.Combine(dir, "nwind.xml"));
            model.WebReport.Report.RegisterData(dataSet, "NorthWind");

            //model.WebReport.SinglePage = true;

            model.WebReport.DesignerPath = "/WebReportDesigner/index.html";
            //model.WebReport.DesignerSaveCallBack = "/SaveDesignedReport";
            model.WebReport.DesignerSaveMethod = (string reportID, string filename, string report) =>
            {
                string webRootPath = _hostingEnvironment.WebRootPath;

                string pathToSave = Path.Combine(webRootPath, "DesignedReports", filename);
                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(Path.GetDirectoryName(pathToSave));

                System.IO.File.WriteAllTextAsync(pathToSave, report);

                return "OK";
            };


            //Uncomment to use ToolbarCustomization
            //ToolbarSettings toolbar = new ToolbarSettings()
            //{
            //    Color = Color.Red,
            //    DropDownMenuColor = Color.Red,
            //    DropDownMenuTextColor = Color.White,
            //    IconColor = IconColors.White,
            //    Position = Positions.Right,
            //    FontSettings = new Font("Arial", 14, FontStyle.Bold),
            //    Exports = new ExportMenuSettings()
            //    {
            //        ExportTypes = Exports.Pdf | Exports.Excel97 | Exports.Rtf
            //    }
            //};
            //model.WebReport.Toolbar = toolbar;



            // Uncomment to use Online Designer
            //model.WebReport.Width = "1000";
            //model.WebReport.Height = "1000";
            //model.WebReport.Mode = WebReportMode.Designer;

            return View(model);
        }

        [HttpGet("prepared/{file}")]
        public IActionResult Prepared(string file)
        {
            var model = new HomeModel()
            {
                WebReport = new WebReport(),
                ReportsList = new[]
                {
                    "Simple List",
                    "Labels",
                    "Master-Detail",
                    "Badges",
                    "Interactive Report, 2-in-1",
                    "Hyperlinks, Bookmarks",
                    "Outline",
                    "Complex (Hyperlinks, Outline, TOC)",
                    "Drill-Down Groups",
                    "Mail Merge"
                },
            };

          

            string dir = FindReportsFolder(Environment.CurrentDirectory);

            model.WebReport.Report.LoadPrepared(file);
            model.WebReport.ReportPrepared = true;


            


            //var dataSet = new DataSet();
            //dataSet.ReadXml(Path.Combine(dir, "nwind.xml"));
            //model.WebReport.Report.RegisterData(dataSet, "NorthWind");


            //model.WebReport.DesignerPath = "/WebReportDesigner/index.html";
            //model.WebReport.DesignerSaveCallBack = "/SaveDesignedReport";
            // Uncomment to use Online Designer
            //model.WebReport.Width = "1000";
            //model.WebReport.Height = "1000";
            //model.WebReport.Mode = WebReportMode.Designer;

            return View("Index",model);
        }

        private string FindReportsFolder(string startDir)
        {
#if DEBUG
            for (int i = 0; i < 6; i++)
            {
                startDir = Path.Combine(startDir, "..");
                string directory = Path.Combine(startDir, "Demos", "Reports");
                if (Directory.Exists(directory))
                    return directory;
            }
#else
            string directory = Path.Combine(startDir, "Reports");
            if (Directory.Exists(directory))
                return directory;

            for (int i = 0; i < 6; i++)
            {
                startDir = Path.Combine(startDir, "..");
                directory = Path.Combine(startDir, "Reports");
                if (Directory.Exists(directory))
                    return directory;
            }
#endif

            throw new Exception("Demos/Reports directory is not found");

        }



        // Call-back for save the designed report 
        [HttpPost("/SaveDesignedReport")]
        public ActionResult SaveDesignedReport(string reportID, [FromQuery(Name = "reportUUID")] string reportName)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;

            Stream reportForSave = Request.Body;
            string pathToSave = Path.Combine(webRootPath, "DesignedReports", reportName);
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(Path.GetDirectoryName(pathToSave));
            using (FileStream file = new FileStream(pathToSave, FileMode.Create))
            {
                reportForSave.CopyToAsync(file);
            }
            return new OkResult();
        }
    }
}
