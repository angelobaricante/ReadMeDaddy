﻿using System;
using System.IO;
using System.Text;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace ReadMeDaddy
{
    public static class FileOperations
    {
        // Method to read text from supported file formats (.txt, .xlsx, .pptx, .docx, .pdf, .csv)
        public static string ReadFile(string filePath)
        {
            try
            {
                string fileExtension = Path.GetExtension(filePath).ToLower();

                switch (fileExtension)
                {
                    case ".txt":
                        return File.ReadAllText(filePath);
                    case ".xlsx":
                        return ReadExcel(filePath);
                    case ".pptx":
                        return ReadPowerPoint(filePath);
                    case ".docx":
                        return ReadWord(filePath);
                    case ".pdf":
                        return ReadPdf(filePath);
                    case ".csv":
                        return ReadCsv(filePath); 
                    default:
                        return "Unsupported file format.";
                }
            }
            catch (Exception ex)
            {
                return $"Error reading file: {ex.Message}";
            }
        }

        // Method to scan supported files (.xlsx, .pptx, .docx, .pdf, .csv)
        public static string ScanFile(string filePath)
        {
            try
            {
                string fileExtension = Path.GetExtension(filePath).ToLower();

                if (fileExtension == ".xlsx" || fileExtension == ".pptx" ||
                    fileExtension == ".docx" || fileExtension == ".pdf" ||
                    fileExtension == ".csv") 
                {
                    return $"File format {fileExtension} is supported for scanning.";
                }
                else
                {
                    return "Unsupported file format for scanning.";
                }
            }
            catch (Exception ex)
            {
                return $"Error scanning file: {ex.Message}";
            }
        }

        // Method to read Excel (.xlsx) files.
        private static string ReadExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; 
                StringBuilder sb = new StringBuilder();

                for (int rowNum = worksheet.Dimension.Start.Row; rowNum <= worksheet.Dimension.End.Row; rowNum++)
                {
                    for (int colNum = worksheet.Dimension.Start.Column; colNum <= worksheet.Dimension.End.Column; colNum++)
                    {
                        var cell = worksheet.Cells[rowNum, colNum];
                        if (cell.Value != null)
                        {
                            sb.Append(cell.Text + " ");
                        }
                    }
                    sb.AppendLine(); 
                }

                return sb.ToString();
            }
        }


        // Method to read PowerPoint (.pptx) files.
        private static string ReadPowerPoint(string filePath)
        {
            StringBuilder sb = new StringBuilder();
            using (PresentationDocument presentationDocument = PresentationDocument.Open(filePath, false))
            {
                var slideParts = presentationDocument.PresentationPart.SlideParts;
                foreach (var slidePart in slideParts)
                {
                    if (slidePart.Slide != null)
                    {
                        var shapes = slidePart.Slide.Descendants<DocumentFormat.OpenXml.Presentation.Shape>();
                        foreach (var shape in shapes)
                        {
                            if (shape.TextBody != null)
                            {
                                var paragraphs = shape.TextBody.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>();
                                foreach (var paragraph in paragraphs)
                                {
                                    var texts = paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>();
                                    foreach (var text in texts)
                                    {
                                        sb.AppendLine(text.Text);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return sb.ToString();
        }

        // Method to read Word (.docx) files.
        private static string ReadWord(string filePath)
        {
            StringBuilder sb = new StringBuilder();
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(filePath, false))
            {
                var paragraphs = wordDocument.MainDocumentPart.Document.Body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>();
                foreach (var para in paragraphs)
                {
                    sb.AppendLine(para.Text);
                }
            }
            return sb.ToString();
        }

        // Method to read PDF files using iText 7.
        private static string ReadPdf(string filePath)
        {
            StringBuilder text = new StringBuilder();
            using (PdfReader reader = new PdfReader(filePath))
            {
                PdfDocument pdfDoc = new PdfDocument(reader);
                int numberOfPages = pdfDoc.GetNumberOfPages();

                for (int i = 1; i <= numberOfPages; i++)
                {
                    string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));
                    text.AppendLine(pageText);
                }
            }
            return text.ToString();
        }

        // Method to read CSV files.
        private static string ReadCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            StringBuilder sb = new StringBuilder();

            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }

            return sb.ToString();
        }
    }
}
