
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClosedXML.Excel;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;

namespace HealthcareInventoryManagementSystem.Infrastructure.Services
{
    public class ExcelReportService : IExcelReportService
    {
        private const string PrimaryColor = "#087F72";
        private const string LightTealColor = "#EAF6F3";
        private const string BorderColor = "#DCE8E6";

        private static readonly string[] MonetaryColumns =
        {
            "Subtotal",
            "TaxAmount",
            "TotalAmount",
            "StockValue",
            "TotalStockValue",
            "TotalValue",
            "UnitPrice",
            "LineTotal"
        };

        private static readonly Dictionary<string, string> CurrencySymbols =
            new(StringComparer.OrdinalIgnoreCase)
            {
                { "PKR", "Rs." },
                { "USD", "$" },
                { "EUR", "€" },
                { "GBP", "£" },
                { "AED", "د.إ" },
                { "SAR", "﷼" }
            };

        public byte[] GenerateExcel<T>(
            IEnumerable<T> data,
            string worksheetName,
            string reportTitle,
            string currencyCode = "PKR",
            decimal exchangeRate = 1m)
        {
            var reportData = data?.ToList()
                ?? new List<T>();

            var properties = typeof(T).GetProperties(
                BindingFlags.Public | BindingFlags.Instance);

            currencyCode = currencyCode.ToUpperInvariant();

            if (!CurrencySymbols.TryGetValue(
                currencyCode,
                out var currencySymbol))
            {
                throw new ArgumentException(
                    "Unsupported currency code.",
                    nameof(currencyCode));
            }

            if (exchangeRate <= 0)
            {
                throw new ArgumentException(
                    "Exchange rate must be greater than zero.",
                    nameof(exchangeRate));
            }

            using var workbook = new XLWorkbook();

            var safeWorksheetName = string.IsNullOrWhiteSpace(worksheetName)
                ? "Report"
                : worksheetName;

            // Excel worksheet names have a maximum length of 31 characters.
            safeWorksheetName = safeWorksheetName
                .Replace("/", "-")
                .Replace("\\", "-")
                .Replace("[", "(")
                .Replace("]", ")")
                .Replace("*", "")
                .Replace("?", "")
                .Replace(":", "-");

            safeWorksheetName = safeWorksheetName.Length > 31
                ? safeWorksheetName[..31]
                : safeWorksheetName;

            var worksheet = workbook.Worksheets.Add(safeWorksheetName);

            var columnCount = Math.Max(properties.Length, 1);

            // Workbook information.
            workbook.Properties.Title = reportTitle;
            workbook.Properties.Subject = "HealthCare IMS Report";
            workbook.Properties.Author = "HealthCare IMS";
            workbook.Properties.Company = "HealthCare IMS";

            // ==========================================
            // REPORT BRANDING
            // ==========================================

            worksheet.Range(1, 1, 1, columnCount).Merge();

            var brandCell = worksheet.Cell(1, 1);
            brandCell.Value = "HealthCare IMS";
            brandCell.Style.Font.Bold = true;
            brandCell.Style.Font.FontSize = 20;
            brandCell.Style.Font.FontColor = XLColor.White;
            brandCell.Style.Fill.BackgroundColor =
                XLColor.FromHtml(PrimaryColor);

            brandCell.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            worksheet.Row(1).Height = 36;

            // Report title.
            worksheet.Range(2, 1, 2, columnCount).Merge();

            var titleCell = worksheet.Cell(2, 1);
            titleCell.Value = string.IsNullOrWhiteSpace(reportTitle)
                ? "Inventory Report"
                : reportTitle;

            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 16;
            titleCell.Style.Font.FontColor =
                XLColor.FromHtml(PrimaryColor);

            titleCell.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            worksheet.Row(2).Height = 30;

            // Subtitle.
            worksheet.Range(3, 1, 3, columnCount).Merge();

            worksheet.Cell(3, 1).Value =
                "INVENTORY MANAGEMENT SYSTEM";

            worksheet.Cell(3, 1).Style.Font.FontSize = 9;
            worksheet.Cell(3, 1).Style.Font.FontColor =
                XLColor.Gray;

            // Generation details.
            worksheet.Range(4, 1, 4, columnCount).Merge();

            worksheet.Cell(4, 1).Value =
                $"Generated: {DateTime.Now:dd-MMM-yyyy HH:mm}     |     Currency: {currencyCode}";

            worksheet.Cell(4, 1).Style.Font.FontSize = 10;
            worksheet.Cell(4, 1).Style.Font.FontColor =
                XLColor.FromHtml(PrimaryColor);

            worksheet.Row(4).Height = 22;

            // ==========================================
            // SUMMARY INFORMATION
            // ==========================================

            worksheet.Cell(6, 1).Value = "TOTAL RECORDS";
            worksheet.Cell(6, 2).Value = reportData.Count;

            worksheet.Cell(6, 4).Value = "SELECTED CURRENCY";
            worksheet.Cell(6, 5).Value = currencyCode;

            var summaryRange = worksheet.Range(6, 1, 6, 5);

            summaryRange.Style.Fill.BackgroundColor =
                XLColor.FromHtml(LightTealColor);

            summaryRange.Style.Font.FontColor =
                XLColor.FromHtml(PrimaryColor);

            summaryRange.Style.Font.Bold = true;

            summaryRange.Style.Alignment.Vertical =
                XLAlignmentVerticalValues.Center;

            summaryRange.Style.Border.OutsideBorder =
                XLBorderStyleValues.Thin;

            summaryRange.Style.Border.OutsideBorderColor =
                XLColor.FromHtml(BorderColor);

            worksheet.Row(6).Height = 25;

            // ==========================================
            // TABLE HEADINGS
            // ==========================================

            const int headerRow = 8;
            const int firstDataRow = 9;

            if (properties.Length > 0)
            {
                for (int column = 0;
                     column < properties.Length;
                     column++)
                {
                    var headerCell = worksheet.Cell(
                        headerRow,
                        column + 1);

                    headerCell.Value = GetColumnLabel(
                        properties[column]);

                    headerCell.Style.Fill.BackgroundColor =
                        XLColor.FromHtml(PrimaryColor);

                    headerCell.Style.Font.Bold = true;
                    headerCell.Style.Font.FontColor = XLColor.White;
                    headerCell.Style.Font.FontSize = 10;

                    headerCell.Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;

                    headerCell.Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Left;

                    headerCell.Style.Border.OutsideBorder =
                        XLBorderStyleValues.Thin;

                    headerCell.Style.Border.OutsideBorderColor =
                        XLColor.FromHtml(BorderColor);
                }

                worksheet.Row(headerRow).Height = 28;
            }

            // ==========================================
            // REPORT DATA
            // ==========================================

            int currentRow = firstDataRow;

            foreach (var item in reportData)
            {
                for (int column = 0;
                     column < properties.Length;
                     column++)
                {
                    var property = properties[column];
                    var value = property.GetValue(item);

                    var cell = worksheet.Cell(
                        currentRow,
                        column + 1);

                    SetCellValue(
                        cell,
                        value,
                        property.Name,
                        currencySymbol,
                        exchangeRate);

                    cell.Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;

                    cell.Style.Border.BottomBorder =
                        XLBorderStyleValues.Thin;

                    cell.Style.Border.BottomBorderColor =
                        XLColor.FromHtml(BorderColor);

                    if ((currentRow - firstDataRow) % 2 == 1)
                    {
                        cell.Style.Fill.BackgroundColor =
                            XLColor.FromHtml("#F7FAFB");
                    }
                }

                worksheet.Row(currentRow).Height = 23;
                currentRow++;
            }

            // ==========================================
            // TABLE FILTERS AND FORMATTING
            // ==========================================

            if (properties.Length > 0)
            {
                var lastDataRow = Math.Max(
                    headerRow,
                    currentRow - 1);

                worksheet.Range(
                    headerRow,
                    1,
                    lastDataRow,
                    properties.Length)
                    .SetAutoFilter();

                // Keep the report headings visible while scrolling.
                worksheet.SheetView.FreezeRows(headerRow);

                // Adjust column widths.
                worksheet.Columns(1, properties.Length)
                    .AdjustToContents();

                for (int column = 1;
                     column <= properties.Length;
                     column++)
                {
                    var excelColumn = worksheet.Column(column);

                    excelColumn.Width = Math.Min(
                        Math.Max(excelColumn.Width + 3, 14),
                        38);
                }
            }
            else
            {
                worksheet.Cell(firstDataRow, 1).Value =
                    "No report columns are available.";
            }

            // General worksheet formatting.
            worksheet.Style.Font.FontName = "Arial";
            worksheet.Style.Font.FontSize = 10;

            worksheet.TabColor =
                XLColor.FromHtml(PrimaryColor);

            // Print settings.
            worksheet.PageSetup.PageOrientation =
                XLPageOrientation.Landscape;

            worksheet.PageSetup.PaperSize =
                XLPaperSize.A4Paper;

            worksheet.PageSetup.FitToPages(1, 0);
            worksheet.PageSetup.PagesWide = 1;

            worksheet.PageSetup.Margins.Top = 0.5;
            worksheet.PageSetup.Margins.Bottom = 0.5;
            worksheet.PageSetup.Margins.Left = 0.3;
            worksheet.PageSetup.Margins.Right = 0.3;

            worksheet.PageSetup.Header.Left.AddText(
                "HealthCare IMS");

            worksheet.PageSetup.Footer.Left.AddText(
                "© HealthCare IMS | Inventory Management System");

            worksheet.PageSetup.Footer.Right.AddText(
                "Page &P of &N");

            // Return Excel file.
            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        private static void SetCellValue(
            IXLCell cell,
            object? value,
            string propertyName,
            string currencySymbol,
            decimal exchangeRate)
        {
            if (value == null)
            {
                cell.Value = "—";
                return;
            }

            if (value is DateTime date)
            {
                cell.Value = date;
                cell.Style.DateFormat.Format = "dd-MMM-yyyy";
                return;
            }

            if (value is DateTimeOffset dateOffset)
            {
                cell.Value = dateOffset.DateTime;
                cell.Style.DateFormat.Format = "dd-MMM-yyyy";
                return;
            }

            if (MonetaryColumns.Contains(
                propertyName,
                StringComparer.OrdinalIgnoreCase)
                && IsNumeric(value))
            {
                var amount = Convert.ToDecimal(value) * exchangeRate;

                cell.Value = (double)amount;

                cell.Style.NumberFormat.Format =
                    $"\"{currencySymbol}\" #,##0.00";

                cell.Style.Font.Bold = true;
                return;
            }

            if (value is bool boolean)
            {
                cell.Value = boolean ? "Active" : "Inactive";
                return;
            }

            if (value is int integer)
            {
                cell.Value = integer;
                return;
            }

            if (value is long longValue)
            {
                cell.Value = longValue;
                return;
            }

            if (value is decimal decimalValue)
            {
                cell.Value = (double)decimalValue;
                cell.Style.NumberFormat.Format = "#,##0.00";
                return;
            }

            if (value is double doubleValue)
            {
                cell.Value = doubleValue;
                cell.Style.NumberFormat.Format = "#,##0.00";
                return;
            }

            if (value is float floatValue)
            {
                cell.Value = (double)floatValue;
                cell.Style.NumberFormat.Format = "#,##0.00";
                return;
            }

            cell.Value = value.ToString() ?? string.Empty;
        }

        private static string GetColumnLabel(PropertyInfo property)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                property.Name,
                "([a-z0-9])([A-Z])",
                "$1 $2");
        }

        private static bool IsNumeric(object value)
        {
            return value is decimal ||
                   value is double ||
                   value is float ||
                   value is int ||
                   value is long;
        }
    }
}
