
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HealthcareInventoryManagementSystem.Infrastructure.Services
{
    public class PdfReportService : IPdfReportService
    {
        // HealthCare IMS theme colors.
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

        public byte[] GeneratePdf<T>(
            IEnumerable<T> data,
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

            var title = string.IsNullOrWhiteSpace(reportTitle)
                ? "Inventory Report"
                : reportTitle;

            var generatedAt = DateTime.Now;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.MarginHorizontal(28);
                    page.MarginVertical(25);

                    page.DefaultTextStyle(style =>
                        style.FontFamily("Lato")
                            .FontSize(8)
                            .FontColor(Colors.Grey.Darken3));

                    // Branded report header.
                    page.Header().Column(header =>
                    {
                        header.Item()
                            .Background(PrimaryColor)
                            .Padding(18)
                            .Column(column =>
                            {
                                column.Item()
                                    .Text("HealthCare IMS")
                                    .FontSize(19)
                                    .Bold()
                                    .FontColor(Colors.White);

                                column.Item()
                                    .PaddingTop(3)
                                    .Text("INVENTORY MANAGEMENT SYSTEM")
                                    .FontSize(8)
                                    .SemiBold()
                                    .FontColor(Colors.White);

                                column.Item()
                                    .PaddingTop(12)
                                    .Text(title)
                                    .FontSize(16)
                                    .Bold()
                                    .FontColor(Colors.White);
                            });

                        header.Item()
                            .PaddingTop(10)
                            .Row(row =>
                            {
                                row.RelativeItem()
                                    .Text($"Generated: {generatedAt:dd-MMM-yyyy HH:mm}")
                                    .FontSize(8)
                                    .FontColor(Colors.Grey.Darken2);

                                row.RelativeItem()
                                    .AlignRight()
                                    .Text($"Currency: {currencyCode}")
                                    .FontSize(8)
                                    .SemiBold()
                                    .FontColor(PrimaryColor);
                            });

                        header.Item()
                            .PaddingTop(8)
                            .LineHorizontal(1)
                            .LineColor(BorderColor);
                    });

                    // Report content.
                    page.Content()
                        .PaddingTop(15)
                        .Column(content =>
                        {
                            // Summary information.
                            content.Item().Row(row =>
                            {
                                row.RelativeItem()
                                    .Background(LightTealColor)
                                    .Padding(12)
                                    .Column(summary =>
                                    {
                                        summary.Item()
                                            .Text("TOTAL RECORDS")
                                            .FontSize(8)
                                            .SemiBold()
                                            .FontColor(PrimaryColor);

                                        summary.Item()
                                            .PaddingTop(4)
                                            .Text(reportData.Count.ToString("N0"))
                                            .FontSize(17)
                                            .Bold()
                                            .FontColor(PrimaryColor);
                                    });

                                row.ConstantItem(12);

                                row.RelativeItem()
                                    .Background(LightTealColor)
                                    .Padding(12)
                                    .Column(summary =>
                                    {
                                        summary.Item()
                                            .Text("REPORT STATUS")
                                            .FontSize(8)
                                            .SemiBold()
                                            .FontColor(PrimaryColor);

                                        summary.Item()
                                            .PaddingTop(4)
                                            .Text("Generated Successfully")
                                            .FontSize(12)
                                            .Bold()
                                            .FontColor(PrimaryColor);
                                    });

                                row.ConstantItem(12);

                                row.RelativeItem()
                                    .Background(LightTealColor)
                                    .Padding(12)
                                    .Column(summary =>
                                    {
                                        summary.Item()
                                            .Text("SELECTED CURRENCY")
                                            .FontSize(8)
                                            .SemiBold()
                                            .FontColor(PrimaryColor);

                                        summary.Item()
                                            .PaddingTop(4)
                                            .Text(currencyCode)
                                            .FontSize(17)
                                            .Bold()
                                            .FontColor(PrimaryColor);
                                    });
                            });

                            content.Item().PaddingTop(18);

                            if (properties.Length == 0)
                            {
                                content.Item()
                                    .AlignCenter()
                                    .Padding(20)
                                    .Text("No report columns are available.")
                                    .FontSize(11)
                                    .FontColor(Colors.Grey.Darken1);
                            }
                            else if (reportData.Count == 0)
                            {
                                content.Item()
                                    .AlignCenter()
                                    .Padding(25)
                                    .Text("No records are available for this report.")
                                    .FontSize(11)
                                    .FontColor(Colors.Grey.Darken1);
                            }
                            else
                            {
                                // Main report table.
                                content.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        foreach (var property in properties)
                                        {
                                            columns.RelativeColumn();
                                        }
                                    });

                                    // Repeated table header on every page.
                                    table.Header(header =>
                                    {
                                        foreach (var property in properties)
                                        {
                                            header.Cell()
                                                .Background(PrimaryColor)
                                                .Border(0.5f)
                                                .BorderColor(BorderColor)
                                                .PaddingVertical(9)
                                                .PaddingHorizontal(6)
                                                .Text(GetColumnLabel(property))
                                                .FontSize(8)
                                                .Bold()
                                                .FontColor(Colors.White);
                                        }
                                    });

                                    // Report records with alternating row backgrounds.
                                    for (int rowIndex = 0;
                                         rowIndex < reportData.Count;
                                         rowIndex++)
                                    {
                                        var item = reportData[rowIndex];

                                        foreach (var property in properties)
                                        {
                                            var value = property.GetValue(item);

                                            if (rowIndex % 2 == 1)
                                            {
                                                // Odd rows: light grey background.
                                                // Background is applied before padding.
                                                table.Cell()
                                                    .Background(Colors.Grey.Lighten4)
                                                    .BorderBottom(0.5f)
                                                    .BorderColor(BorderColor)
                                                    .PaddingVertical(8)
                                                    .PaddingHorizontal(6)
                                                    .Text(
                                                        FormatValue(
                                                            value,
                                                            property.Name,
                                                            currencySymbol,
                                                            exchangeRate))
                                                    .FontSize(7);
                                            }
                                            else
                                            {
                                                // Even rows: white background.
                                                table.Cell()
                                                    .BorderBottom(0.5f)
                                                    .BorderColor(BorderColor)
                                                    .PaddingVertical(8)
                                                    .PaddingHorizontal(6)
                                                    .Text(
                                                        FormatValue(
                                                            value,
                                                            property.Name,
                                                            currencySymbol,
                                                            exchangeRate))
                                                    .FontSize(7);
                                            }
                                        }
                                    }
                                });
                            }
                        });

                    // Professional footer.
                    page.Footer()
                        .PaddingTop(10)
                        .BorderTop(1)
                        .BorderColor(BorderColor)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .AlignMiddle()
                                .Text("© HealthCare IMS | Inventory Management System")
                                .FontSize(8)
                                .FontColor(Colors.Grey.Darken1);

                            row.RelativeItem()
                                .AlignRight()
                                .AlignMiddle()
                                .Text(text =>
                                {
                                    text.Span("Page ")
                                        .FontSize(8);

                                    text.CurrentPageNumber()
                                        .FontSize(8);

                                    text.Span(" of ")
                                        .FontSize(8);

                                    text.TotalPages()
                                        .FontSize(8);
                                });
                        });
                });
            });

            return document.GeneratePdf();
        }

        private static string GetColumnLabel(PropertyInfo property)
        {
            // Convert property names such as TotalQuantity
            // into readable column headings.
            var name = property.Name;

            return System.Text.RegularExpressions.Regex.Replace(
                name,
                "([a-z0-9])([A-Z])",
                "$1 $2");
        }

        private static string FormatValue(
            object? value,
            string propertyName,
            string currencySymbol,
            decimal exchangeRate)
        {
            if (value == null)
            {
                return "—";
            }

            if (value is DateTime date)
            {
                return date.ToString("dd-MMM-yyyy");
            }

            if (value is DateTimeOffset dateOffset)
            {
                return dateOffset.ToString("dd-MMM-yyyy");
            }

            if (MonetaryColumns.Contains(
                propertyName,
                StringComparer.OrdinalIgnoreCase)
                && IsNumeric(value))
            {
                var amount = Convert.ToDecimal(value) * exchangeRate;

                return $"{currencySymbol} {amount:N2}";
            }

            if (value is decimal decimalValue)
            {
                return decimalValue.ToString("N2");
            }

            if (value is double doubleValue)
            {
                return doubleValue.ToString("N2");
            }

            if (value is float floatValue)
            {
                return floatValue.ToString("N2");
            }

            if (value is bool boolean)
            {
                return boolean ? "Active" : "Inactive";
            }

            return value.ToString() ?? "—";
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
