using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;
using X15 = DocumentFormat.OpenXml.Office2013.Excel;
using OTTR.Models;
using System.Threading.Tasks;
using OTTR.Controllers;
using Microsoft.Extensions.Configuration;
using ClosedXML.Excel;

namespace OTTR.Services
{
    public interface IExcelFileGenerator
    {
        string CreateExcelFile(List<OttrMonitoringModel> data, string OutPutFileDirectory);
    }
    public class ExcelFileGenerator : IExcelFileGenerator
    {
        private readonly IConfiguration configuration;

        public ExcelFileGenerator(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateExcelFile(List<OttrMonitoringModel> data, string OutPutFileDirectory)
        {
            var datetime = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_");

            string fileFullname = Path.Combine(OutPutFileDirectory, "Output.xlsx");

            if (File.Exists(fileFullname))
            {
                fileFullname = Path.Combine(OutPutFileDirectory, "Output_" + datetime + ".xlsx");
            }

            using (SpreadsheetDocument package = SpreadsheetDocument.Create(fileFullname, SpreadsheetDocumentType.Workbook))
            {
                CreatePartsForExcel(package, data);
            }
            //return fileFullname;
            return handleDateinExcel(fileFullname);
        }

        private void CreatePartsForExcel(SpreadsheetDocument document, List<OttrMonitoringModel> data)
        {
            SheetData partSheetData = GenerateSheetdataForDetailsAsync(data).Result;

            WorkbookPart workbookPart1 = document.AddWorkbookPart();
            GenerateWorkbookPartContent(workbookPart1);

            WorkbookStylesPart workbookStylesPart1 = workbookPart1.AddNewPart<WorkbookStylesPart>("rId3");
            GenerateWorkbookStylesPartContent(workbookStylesPart1);

            WorksheetPart worksheetPart1 = workbookPart1.AddNewPart<WorksheetPart>("rId1");
            GenerateWorksheetPartContent(worksheetPart1, partSheetData);
        }

        private void GenerateWorkbookStylesPartContent(WorkbookStylesPart workbookStylesPart1)
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            Fonts fonts1 = new Fonts() { Count = (UInt32Value)2U, KnownFonts = true };

            Font font1 = new Font();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            Color color1 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);

            Font font2 = new Font();
            Bold bold1 = new Bold();
            FontSize fontSize2 = new FontSize() { Val = 11D };
            Color color2 = new Color() { Theme = (UInt32Value)1U };
            FontName fontName2 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme2 = new FontScheme() { Val = FontSchemeValues.Minor };

            font2.Append(bold1);
            font2.Append(fontSize2);
            font2.Append(color2);
            font2.Append(fontName2);
            font2.Append(fontFamilyNumbering2);
            font2.Append(fontScheme2);

            fonts1.Append(font1);
            fonts1.Append(font2);

            Fills fills1 = new Fills() { Count = (UInt32Value)2U };

            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };

            fill1.Append(patternFill1);

            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };

            fill2.Append(patternFill2);

            fills1.Append(fill1);
            fills1.Append(fill2);

            Borders borders1 = new Borders() { Count = (UInt32Value)2U };

            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            Border border2 = new Border();

            LeftBorder leftBorder2 = new LeftBorder() { Style = BorderStyleValues.Thin };
            Color color3 = new Color() { Indexed = (UInt32Value)64U };

            leftBorder2.Append(color3);

            RightBorder rightBorder2 = new RightBorder() { Style = BorderStyleValues.Thin };
            Color color4 = new Color() { Indexed = (UInt32Value)64U };

            rightBorder2.Append(color4);

            TopBorder topBorder2 = new TopBorder() { Style = BorderStyleValues.Thin };
            Color color5 = new Color() { Indexed = (UInt32Value)64U };

            topBorder2.Append(color5);

            BottomBorder bottomBorder2 = new BottomBorder() { Style = BorderStyleValues.Thin };
            Color color6 = new Color() { Indexed = (UInt32Value)64U };

            bottomBorder2.Append(color6);
            DiagonalBorder diagonalBorder2 = new DiagonalBorder();

            border2.Append(leftBorder2);
            border2.Append(rightBorder2);
            border2.Append(topBorder2);
            border2.Append(bottomBorder2);
            border2.Append(diagonalBorder2);

            borders1.Append(border1);
            borders1.Append(border2);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)3U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyBorder = true };
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)1U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = true };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

            stylesheetExtension1.Append(slicerStyles1);

            StylesheetExtension stylesheetExtension2 = new StylesheetExtension() { Uri = "{9260A510-F301-46a8-8635-F512D64BE5F5}" };
            stylesheetExtension2.AddNamespaceDeclaration("x15", "http://schemas.microsoft.com/office/spreadsheetml/2010/11/main");
            X15.TimelineStyles timelineStyles1 = new X15.TimelineStyles() { DefaultTimelineStyle = "TimeSlicerStyleLight1" };

            stylesheetExtension2.Append(timelineStyles1);

            stylesheetExtensionList1.Append(stylesheetExtension1);
            stylesheetExtensionList1.Append(stylesheetExtension2);

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);

            workbookStylesPart1.Stylesheet = stylesheet1;
        }

        private void GenerateWorkbookPartContent(WorkbookPart workbookPart1)
        {
            Workbook workbook1 = new Workbook();
            Sheets sheets1 = new Sheets();
            Sheet sheet1 = new Sheet() { Name = "OTTR-Sheet", SheetId = (UInt32Value)1U, Id = "rId1" };
            sheets1.Append(sheet1);
            workbook1.Append(sheets1);
            workbookPart1.Workbook = workbook1;
        }

        private void GenerateWorksheetPartContent(WorksheetPart worksheetPart1, SheetData sheetData1)
        {
            Worksheet worksheet1 = new Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            
            worksheet1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            SheetDimension sheetDimension1 = new SheetDimension() { Reference = "A1" };

            SheetViews sheetViews1 = new SheetViews();

            SheetView sheetView1 = new SheetView() { TabSelected = true, WorkbookViewId = (UInt32Value)0U };
            Selection selection1 = new Selection() { ActiveCell = "A1", SequenceOfReferences = new ListValue<StringValue>() { InnerText = "A1" } };

            sheetView1.Append(selection1);

            sheetViews1.Append(sheetView1);
            SheetFormatProperties sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, DyDescent = 0.25D };

            PageMargins pageMargins1 = new PageMargins() { Left = 0.7D, Right = 0.7D, Top = 0.75D, Bottom = 0.75D, Header = 0.3D, Footer = 0.3D };
            worksheet1.Append(sheetDimension1);
            worksheet1.Append(sheetViews1);
            worksheet1.Append(sheetFormatProperties1);
            worksheet1.Append(sheetData1);
            worksheet1.Append(pageMargins1);
            worksheetPart1.Worksheet = worksheet1;
        }

        private async Task<SheetData> GenerateSheetdataForDetailsAsync(List<OttrMonitoringModel> data)
        {
            SheetData sheetData1 = new SheetData();
            sheetData1.Append(CreateHeaderRowForExcel());

            foreach (var ottrMonitoring in data)
            {
               
                    if (string.IsNullOrEmpty(ottrMonitoring.FOPMId))
                    {
                        Row partsRows = GenerateRowForChildPartDetail(ottrMonitoring);
                        sheetData1.Append(partsRows);
                     }
                    else
                    {
                        string[] ListOfEID = ottrMonitoring.FOPMId.Split(";");
                        if(ListOfEID.Length == 1) {
                            Row partsRows = GenerateRowForChildPartDetail(ottrMonitoring);
                            sheetData1.Append(partsRows);
                        }
                        else
                        {
                            string [] ListOfFOPMEmailId = ottrMonitoring.FOPMEmailId.Split(";");
                            string[] ListOfFOPMName = ottrMonitoring.FOPMName.Split(";");
                            for (int i = 0; i <= ListOfEID.Length-1; i++)
                            {
                                    ottrMonitoring.FOPMId = ListOfEID[i];
                                    ottrMonitoring.FOPMEmailId = ListOfFOPMEmailId[i];
                                    ottrMonitoring.FOPMName = ListOfFOPMName[i];
                                    Row partsRows = GenerateRowForChildPartDetail(ottrMonitoring);
                                    sheetData1.Append(partsRows);
                            }

                        }
                    }
                
            }
            return sheetData1;
        }

        private Row CreateHeaderRowForExcel()
        {
            Row workRow = new Row();
            workRow.Append(CreateCell("OTTR ID", 2U));
            workRow.Append(CreateCell("GES Center", 2U));
            workRow.Append(CreateCell("GES LOB", 2U));
            workRow.Append(CreateCell("GES PM / EM EID", 2U));
            workRow.Append(CreateCell("GES PM / EM Name", 2U));
            workRow.Append(CreateCell("Project ID", 2U));
            workRow.Append(CreateCell("Project Name", 2U));
            workRow.Append(CreateCell("Deliverables / Milestone", 2U));
           workRow.Append(CreateCell("Month", 2U));
            workRow.Append(CreateCell("Base Date", 2U));
            workRow.Append(CreateCell("Plan Date", 2U));
            workRow.Append(CreateCell("Actual Date", 2U));
            workRow.Append(CreateCell("Actual Date Modified On", 2U));
            workRow.Append(CreateCell("GES Group", 2U));
            workRow.Append(CreateCell("GES Delivery Group", 2U));
            workRow.Append(CreateCell("Delivery Hours", 2U));
            workRow.Append(CreateCell("Rework Hours", 2U));
            workRow.Append(CreateCell("Rework %", 2U));            
            workRow.Append(CreateCell("Survey Request", 2U));
            workRow.Append(CreateCell("Pole", 2U));
            workRow.Append(CreateCell("Region", 2U));
            workRow.Append(CreateCell("Country", 2U));
            workRow.Append(CreateCell("Survey type", 2U));
            workRow.Append(CreateCell("GES Resource EID", 2U));
            workRow.Append(CreateCell("GES Resource Name", 2U));
            workRow.Append(CreateCell("Product / Platform", 2U));
            workRow.Append(CreateCell("Difference", 2U));
            workRow.Append(CreateCell("Attribute to", 2U));
           workRow.Append(CreateCell("Tracking to Design Input", 2U));
           workRow.Append(CreateCell("Internal / External Attribute", 2U));
           workRow.Append(CreateCell("Comments / Remarks", 2U));
            workRow.Append(CreateCell("Survey For", 2U));
            workRow.Append(CreateCell("External Customer Name", 2U));
            workRow.Append(CreateCell("External Custmer EmailId", 2U));
            workRow.Append(CreateCell("FO PM EID", 2U));
            workRow.Append(CreateCell("FO PM Name", 2U));
            workRow.Append(CreateCell("FO PM EmailId", 2U));
            workRow.Append(CreateCell("Created By", 2U));
            workRow.Append(CreateCell("Created On", 2U));
            workRow.Append(CreateCell("Created Email", 2U));
            workRow.Append(CreateCell("Created Manager Email", 2U));
            workRow.Append(CreateCell("GESPM Email", 2U));
            workRow.Append(CreateCell("GESPM Manager Email", 2U));

            return workRow;
        }

        private Row GenerateRowForChildPartDetail(OttrMonitoringModel ottrMonitoring)
        {
            Row tRow = new Row();
            int diffrence = 0;
            if (ottrMonitoring.ActualDate == null)
            {
                diffrence = 0;
            }
            else
            {
                if (ottrMonitoring.PlanDate != null)
                {
                    diffrence = (int)(BusinessDaysUntil(ottrMonitoring.PlanDate, ottrMonitoring.ActualDate.Value, new DateTime[0]));
                }
                else
                {
                    diffrence = 0;
                }
            }

            tRow.Append(CreateCell(ottrMonitoring.OttrId.ToString(), "int"));
            tRow.Append(CreateCell(ottrMonitoring.GESCenter.ToString().Split('-')[0].ToString(), "string"));
            tRow.Append(CreateCell(ottrMonitoring.GESCenter.ToString().Split('-')[1].ToString(), "string"));
            tRow.Append(CreateCell(ottrMonitoring.gespmEID != null ? ottrMonitoring.gespmEID.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.GESPM != null ? ottrMonitoring.GESPM.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.ProjectId != null ? " " + ottrMonitoring.ProjectId.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.ProjectName != null ? ottrMonitoring.ProjectName.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.Deliverables != null ? ottrMonitoring.Deliverables.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.Month != null ? getFullMonth(ottrMonitoring.Month, ottrMonitoring.PlanDate, ottrMonitoring.BaseDate) : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.BaseDate.ToString("dd-MMM-yy"), "string"));
            tRow.Append(CreateCell(ottrMonitoring.PlanDate.ToString("dd-MMM-yy"), "string"));
            if (ottrMonitoring.ActualDate == null)
            {
                tRow.Append(CreateCell("", "string"));
            }
            else
            {
                tRow.Append(CreateCell(ottrMonitoring.ActualDate.Value.ToString("dd-MMM-yy"), "string"));
            }
            tRow.Append(CreateCell(ottrMonitoring.ActualDateModifiedOn != null ? ottrMonitoring.ActualDateModifiedOn.Value.ToString("dd-MMM-yy") : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.GESGroup != null ? ottrMonitoring.GESGroup.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.Region != null ? ottrMonitoring.Region.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.DeliveryHours != null ? ottrMonitoring.DeliveryHours.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.ReworkHours != null ? ottrMonitoring.ReworkHours.ToString() : "", "string"));
            //tRow.Append(CreateCell((ottrMonitoring.ReworkHours != null && ottrMonitoring.DeliveryHours != null) ? (Math.Round((Convert.ToDecimal(ottrMonitoring.ReworkHours) / Convert.ToDecimal(ottrMonitoring.DeliveryHours)) * 100, 0)).ToString() : "", "string"));
            tRow.Append(CreateCell((ottrMonitoring.ReworkHours != null && ottrMonitoring.DeliveryHours != null && ottrMonitoring.DeliveryHours != 0) ? (Math.Round((Convert.ToDecimal(ottrMonitoring.ReworkHours) / Convert.ToDecimal(ottrMonitoring.DeliveryHours)) * 100, 0)).ToString() : "","string"));
            
            tRow.Append(CreateCell(ottrMonitoring.SurveyRequest != null ? ottrMonitoring.SurveyRequest.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.SpendPlan_Pole != null ? ottrMonitoring.SpendPlan_Pole.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.SpendPlan_Region != null ? ottrMonitoring.SpendPlan_Region.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.SpendPlan_Country != null ? ottrMonitoring.SpendPlan_Country.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.surveyType != null ? ottrMonitoring.surveyType.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.gesResourceId != null ? ottrMonitoring.gesResourceId.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.gesResourceName != null ? ottrMonitoring.gesResourceName.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.Product != null ? ottrMonitoring.Product.ToString() : "", "string"));
            tRow.Append(CreateCell(diffrence.ToString(), "int"));
            tRow.Append(CreateCell(ottrMonitoring.AttributeTo != null ? ottrMonitoring.AttributeTo.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.TrackingToDesignInput != null ? ottrMonitoring.TrackingToDesignInput.ToString().Replace(",", ", ") : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.InternalExternalAttributeId != null ? ottrMonitoring.InternalExternalAttributeId.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.Comments != null ? ottrMonitoring.Comments.ToString().Replace(",", ", ") : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.surveyFor!= null ? ottrMonitoring.surveyFor.ToString().Replace(",", ", ") : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.exCustName != null ? ottrMonitoring.exCustName.ToString().Replace(",", ", ") : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.exCustEmailId != null ? ottrMonitoring.exCustEmailId.ToString().Replace(",", ", ") : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.FOPMId != null ? ottrMonitoring.FOPMId.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.FOPMName != null ? ottrMonitoring.FOPMName.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.FOPMEmailId != null ? ottrMonitoring.FOPMEmailId.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.CreatedBy != null ? ottrMonitoring.CreatedBy.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.CreatedOn.ToString("dd-MMM-yy"), "string"));
            tRow.Append(CreateCell(ottrMonitoring.CreatedByEmail != null ? ottrMonitoring.CreatedByEmail.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.CreatedByManagerEmailId != null ? ottrMonitoring.CreatedByManagerEmailId.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.GESPMEmail != null ? ottrMonitoring.GESPMEmail.ToString() : "", "string"));
            tRow.Append(CreateCell(ottrMonitoring.GESPMManagerEmail != null ? ottrMonitoring.GESPMManagerEmail.ToString() : "", "string"));
            return tRow;
        }

        private string getFullMonth(string month,DateTime plandate,DateTime basedate)
        {

            try
            {
                int iMonthNo = Convert.ToDateTime("01-" + month + "-2011").Month;
                int pyear = 0;
                try { pyear = plandate.Year; } catch { }
                if (pyear == 0) {
                    try { pyear = basedate.Year; } catch { }
                }
                DateTime dtDate = new DateTime(2000, iMonthNo, 1);
                return dtDate.ToString("MMM")+"-"+ pyear;
            }
            catch { return month; }

        }

        private Cell CreateCell(string text, string type)
        {
            Cell cell = new Cell();
            cell.StyleIndex = 1U;
            cell.DataType = ResolveCellDataTypeOnValue(text, type);
            cell.CellValue = new CellValue(text);
            //cell.Style.NumberFormat.Format = "dd-mmm-yy";
            return cell;
        }

        private Cell CreateCell(string text, uint styleIndex)
        {
            Cell cell = new Cell();
            cell.StyleIndex = styleIndex;
            cell.DataType = ResolveCellDataTypeOnValue(text, "string");
            
            cell.CellValue = new CellValue(text);
            return cell;
        }

        private EnumValue<CellValues> ResolveCellDataTypeOnValue(string text, string type)
        {
            
            int intVal;
            double doubleVal;
            if (int.TryParse(text, out intVal) ||  double.TryParse(text, out doubleVal))
            {
                return CellValues.Number;
            }
            //if (type.Equals("date") && !String.IsNullOrEmpty(text))
            //{
            //    return CellValues.Date;
            //}
            return CellValues.String;
        }


        public async Task<Dictionary<string, string>> getUserDetails(string EID)
        {
            if (!String.IsNullOrEmpty(EID))
            {
                UserPeopleSoftController user = new UserPeopleSoftController(configuration);
                var userName = await user.GetUserInformationForUpload(EID) as Microsoft.AspNetCore.Mvc.OkObjectResult;
                if (userName != null)
                {
                    Newtonsoft.Json.Linq.JObject result = (Newtonsoft.Json.Linq.JObject)userName.Value;
                    //if (result != null && result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["numrows"].ToString() != "0")
                    //{
                    //    Dictionary<string, string> UserDetails = new Dictionary<string, string>();
                    //    string firstName = result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.PREF_FIRST_NAME"].ToString();
                    //    string lastName = result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.LAST_NAME"].ToString();
                    //    UserDetails.Add("UserName", firstName + " " + lastName);
                    //    UserDetails.Add("EmailId", result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.EMAIL_ADDR"].ToString());
                    //    return UserDetails;
                    //}
                    if (result != null && result["data"]?["query"]?["numrows"]?.ToString() != "0")
                    {
                        var rows = result["data"]["query"]["rows"];
                        Dictionary<string, string> UserDetails = new Dictionary<string, string>();
                        string firstName = rows[0]["PREF_FIRST_NAME"].ToString();
                        string lastName = rows[0]["LAST_NAME"].ToString();
                        UserDetails.Add("UserName", firstName + " " + lastName);
                        UserDetails.Add("EmailId", rows[0]["EMAIL_ADDR"].ToString());
                        //UserDetails.Add("EmailId", result["QAS_GETQUERYRESULTS_RESP_MSG"]["query"]["row"]["A.EMAIL_ADDR"].ToString());
                        return UserDetails;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public string handleDateinExcel(string path)
        {
            using (XLWorkbook workBook = new XLWorkbook(path))
            {
                IXLWorksheet workSheet = workBook.Worksheet(1);
                foreach (IXLRow row in workSheet.Rows())
                {
                    foreach (IXLCell cell in row.Cells())
                    {
                        DateTime datetime;
                        string[] format = new string[] { "dd-MMM-yy" };
                        if (DateTime.TryParseExact(cell.Value.ToString(), format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.NoCurrentDateDefault, out datetime))
                        {
                            cell.Value = datetime;
                            cell.Style.NumberFormat.Format = "dd-mmm-yy";
                        }
                    }
                }
                workSheet.Columns().AdjustToContents();
                workBook.Save(true);
            }
            return path;
        }
        private int BusinessDaysUntil(DateTime firstDay, DateTime lastDay, params DateTime[] holidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            bool negativeNumber = false;
            if (firstDay > lastDay)
            {
                negativeNumber = true;
                DateTime temp = firstDay.Date;
                firstDay = lastDay.Date;
                lastDay = temp;
            }
            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday
                    ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday
                    ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of holidays during the time interval
            foreach (DateTime holiday in holidays)
            {
                DateTime bh = holiday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return negativeNumber ? businessDays : businessDays * -1;
        }
    }
}