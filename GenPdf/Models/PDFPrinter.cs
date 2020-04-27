using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GenPdf
{
    public class PDFPrinter
    {
        string _baseDirectory;
        string _rootDir;
        List<SelectListItem> _files = new List<SelectListItem>();
       // public int ID = 0;
        static string _lastFilePath;
        string _directory;
        string _file = ".pdf";
        string _printerName = "Microsoft Print to PDF";
        Color _rectangleColor;
        Color _textColor;
        public string Text { get; set; }
        string _sentence;
        public string Sentence { get => _sentence; set => _sentence = value; }
        public string TextColor { get; set; }
        public string RectangleColor { get; set; }
        public static string LastFilePath { get => _lastFilePath; }
        public List<SelectListItem> ColorList { get => _colorList; }
        public string SelectedDrawing { get; set; }
        public List<SelectListItem> Files { get => _files;  }

        List<SelectListItem> _colorList = new List<SelectListItem>
                          {
                             new SelectListItem{ Text="Crimson", Value = "0" },
                             new SelectListItem{ Text="Black", Value = "1" },
                             new SelectListItem{ Text="Red", Value = "2" },
                             new SelectListItem{ Text="Green", Value = "3" },
                             new SelectListItem{ Text="Blue", Value = "4" },
                             new SelectListItem{ Text="Yellow", Value = "5" },
                             new SelectListItem{ Text="Orange", Value = "6" },
                             new SelectListItem{ Text="Salmon", Value = "7" },
                             new SelectListItem{ Text="Purple", Value = "8" },
                             new SelectListItem{ Text="Indigo", Value = "9" },
                             new SelectListItem{ Text="Olive", Value = "10" },
                             new SelectListItem{ Text="Silver", Value = "11" },
                             new SelectListItem{ Text="Gold", Value = "12" },
                          };

        internal void SelecFile(string guidOfFile)
        {
            MakeLastFile(guidOfFile);
        }
        private void MakeLastFile(string documentName, bool createDir=false)
        {
            _directory = _rootDir + documentName;
            if (createDir == true)
            {
                Directory.CreateDirectory(_directory);
            }
            _lastFilePath = Path.Combine(_directory, 0 + _file);
        }

        public PDFPrinter()
        {
            _baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            _baseDirectory = _baseDirectory.Remove(0, "file:\\".Length);
            _rootDir= _baseDirectory + "\\" + _printerName + "\\";
            _sentence = "Curiosity killed the cat";
            TextColor = "Green";
            RectangleColor = "Yellow";
            FillFiles();
        }

        private void FillFiles()
        {
            var roots=Directory.EnumerateDirectories(_rootDir);
            int index = 0;
            foreach(var root in roots)
            {
                var file=Directory.EnumerateFiles(root);
                int id = int.Parse(Path.GetFileNameWithoutExtension(file.ElementAt(0)));
                string documentName = Path.GetFileNameWithoutExtension(root);
                _files.Add(new SelectListItem(index.ToString(),documentName));
                index++;
            }
        }

        public void Print(string documentName)
        {

            SetColors();
            SetText();
            if (string.IsNullOrEmpty(documentName) == true)
            {

                documentName = Guid.NewGuid().ToString();
                _files.Add(new SelectListItem(_files.Count.ToString(), documentName));
                MakeLastFile(documentName, true);

            }
                PrintDocument printDocument = new PrintDocument()
                {
                    DocumentName = documentName,
                    PrinterSettings = new PrinterSettings()
                    {
                        // set the printer to 'Microsoft Print to PDF'
                        PrinterName = _printerName,

                        // tell the object this document will print to file
                        PrintToFile = true,

                        // set the filename to whatever you like (full path)
                        PrintFileName = _lastFilePath// Path.Combine(_directory, _file)
                    }
                };

                Console.WriteLine(printDocument.PrinterSettings.PrintFileName);
                printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
                printDocument.Print();
               // ID++;
            
        }

      
        private void SetText()
        {
            string timeStamp = DateTime.UtcNow.Subtract(new DateTime()).ToString();
            Text = String.Format("{0}:  {1}", timeStamp, _sentence);
        }

        private void SetColors()// sets the compile time colors
        {
            try //try getting the known colors from the colors strings
            {
                if (RectangleColor != null && TextColor != null)
                {
                    KnownColor kcRect = (KnownColor)Enum.Parse(typeof(KnownColor), RectangleColor);
                    KnownColor kcText = (KnownColor)Enum.Parse(typeof(KnownColor), TextColor);
                    _rectangleColor = Color.FromKnownColor(kcRect);
                    _textColor = Color.FromKnownColor(kcText);
                }
            }
            catch (Exception ex)
            {
                //on error reverts to black
                Debug.WriteLine("Color input error");
            }

        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs ev)
        {
           

            // Create pen.
            Pen pen = new Pen(_rectangleColor, 1);

          
            SetShape(ev, pen, true);
            DrawText(ev);
        }

        private void DrawText(PrintPageEventArgs ev)
        {
            ev.Graphics.DrawString(this.Text, new Font("Arial", 12), new SolidBrush(_textColor), ev.MarginBounds.Left,ev.MarginBounds.Top, new StringFormat());
        }

        private static void SetShape(PrintPageEventArgs ev, Pen pen, bool ellipse)
        {
            Rectangle rectAll = new Rectangle(ev.PageBounds.Left, ev.PageBounds.Top, ev.PageBounds.Right - ev.PageBounds.Left, ev.PageBounds.Bottom - ev.PageBounds.Top);
            Rectangle rect = new Rectangle(ev.MarginBounds.Left, ev.MarginBounds.Top, ev.MarginBounds.Right - ev.MarginBounds.Left, ev.MarginBounds.Bottom - ev.MarginBounds.Top);

            ev.Graphics.FillRectangle(Brushes.Black, rectAll);
            //if (ellipse == false)
            //{
            ev.Graphics.DrawRectangle(pen, rect);
            //}
            //else
            //{
                ev.Graphics.DrawEllipse(pen, rect);
            //}
        }
    }


}
