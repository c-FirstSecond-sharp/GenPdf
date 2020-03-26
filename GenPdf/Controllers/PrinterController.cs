using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GenPdf
{
    public class PrinterController : Controller
    {
       
        public PrinterController()
        {
           
        }
        // GET: Printer
        public ActionResult Index()
        {
            return GetPDF();
            // return View();
        }

        private ActionResult GetPDF()
        {
            if (string.IsNullOrEmpty(PDFPrinter.LastFilePath) == false)
            {
                FileStream fs = new FileStream(PDFPrinter.LastFilePath, FileMode.Open, FileAccess.Read);
                return File(fs, "application/pdf");
            }
            else
            {
                return new EmptyResult();
            }
        }
        
        // GET: Printer
        public ActionResult PDFView()
        {
            //frame1.Attributes.Add("src", "@Url.Action('GetPDF')");
            return View(Startup.PDFPrinter);
        }

        // GET: Printer/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Printer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Printer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                
                return SetupAndPrint(collection);
                //return RedirectToAction(nameof(V));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        private  ActionResult SetupAndPrint(IFormCollection collection)
        {
            Startup.PDFPrinter.Sentence = collection[nameof(Startup.PDFPrinter.Sentence)];
            Startup.PDFPrinter.TextColor = Startup.PDFPrinter.ColorList.ElementAt(int.Parse(collection[nameof(Startup.PDFPrinter.TextColor)])).Text;
            Startup.PDFPrinter.RectangleColor = Startup.PDFPrinter.ColorList.ElementAt(int.Parse(collection[nameof(Startup.PDFPrinter.RectangleColor)])).Text;
            Startup.PDFPrinter.Print();
            return RedirectToAction(nameof(PDFView));
        }

        // GET: Printer/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Printer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
               return SetupAndPrint(collection);
                //return RedirectToAction(nameof(V));
            }
            catch
            {
                return View();
            }
        }

        // GET: Printer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Printer/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}