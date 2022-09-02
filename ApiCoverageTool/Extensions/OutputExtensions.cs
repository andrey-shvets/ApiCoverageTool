using System;
using System.Globalization;
using System.IO;
using System.Linq;
using ApiCoverageTool.Models;
using ClosedXML.Excel;
using CsvHelper;

namespace ApiCoverageTool.Extensions;

public static class OutputExtensions
{
    /// <summary>
    /// Creates a table with mapping data in file.
    /// </summary>
    /// <param name="apiCoverageResult">Api mapping data.</param>
    /// <param name="path">Path to the *.csv file. Overrides existing file.</param>
    public static void ToCsv(this ApiCoverageResult apiCoverageResult, string path)
    {
        if (apiCoverageResult is null)
            throw new ArgumentNullException(nameof(apiCoverageResult), $"{nameof(apiCoverageResult)} can not be null");

        if (path is null)
            throw new ArgumentNullException(nameof(path), $"{nameof(path)} can not be null");

        if (File.Exists(path))
            File.Delete(path);

        var endpointsMapping = apiCoverageResult.EndpointsMapping
            .OrderByDescending(m => m.Value.Any())
            .ThenBy(m => m.Key.Path)
            .Select(m => new { Method = m.Key.RestMethod, Endpoint = m.Key.Path, TestsCount = m.Value.Count })
            .ToList();

        using var writer = new StreamWriter(path);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csvWriter.WriteRecords(endpointsMapping);
    }

    /// <summary>
    /// Creates Excel table with mapping data in *.xlsx file. If file already exists creates new worksheet. If worksheet with the provided name already exists, it will be overriden.
    /// </summary>
    /// <param name="apiCoverageResult">Api mapping data.</param>
    /// <param name="path">Path to the excel file. Required to have *.xlsx extension.</param>
    /// <param name="worksheetName">Name of the worksheet where will be created the table with mapping data.</param>
    /// <param name="deleteFileIfExists">If set to true existing file will be deleted and new one will be created.</param>
    public static void ToXlsx(this ApiCoverageResult apiCoverageResult, string path, string worksheetName, bool deleteFileIfExists = false)
    {
        ValidateInputParameters(apiCoverageResult, path, worksheetName);

        if (deleteFileIfExists && File.Exists(path))
            File.Delete(path);

        using var workbook = File.Exists(path) ? new XLWorkbook(path) : new XLWorkbook();

        if (workbook.Worksheets.Contains(worksheetName))
            workbook.Worksheets.Delete(worksheetName);

        var worksheet = workbook.Worksheets.Add(worksheetName);

        worksheet.AddHeaders();
        worksheet.FillCoverageData(apiCoverageResult);
        worksheet.FillCoverageMetrics();

        workbook.SaveAs(path);
    }

    private static void ValidateInputParameters(ApiCoverageResult apiCoverageResult, string path, string worksheetName)
    {
        if (apiCoverageResult is null)
            throw new ArgumentNullException(nameof(apiCoverageResult), $"{nameof(apiCoverageResult)} can not be null");

        if (path is null)
            throw new ArgumentNullException(nameof(path), $"{nameof(path)} can not be null");

        if (worksheetName is null)
            throw new ArgumentNullException(nameof(path), $"{nameof(worksheetName)} can not be null");
    }

    private static void AddHeaders(this IXLWorksheet worksheet)
    {
        worksheet.Cell("A1").Value = "Method";
        worksheet.Cell("B1").Value = "Endpoint";
        worksheet.Cell("C1").Value = "Tests count";

        var rngHeaders = worksheet.Range("A1:C1");
        rngHeaders.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        rngHeaders.Style.Font.Bold = true;
        rngHeaders.Style.Font.FontColor = XLColor.DarkBlue;
        rngHeaders.Style.Fill.BackgroundColor = XLColor.SkyBlue;
    }

    private static void FillCoverageData(this IXLWorksheet worksheet, ApiCoverageResult apiCoverageResult)
    {
        var endpointsMapping = apiCoverageResult.EndpointsMapping
            .OrderByDescending(m => m.Value.Any())
            .ThenBy(m => m.Key.Path)
            .Select(m => new { Method = m.Key.RestMethod, Endpoint = m.Key.Path, TestsCount = m.Value.Count })
            .ToList();

        var rowIndex = 2;

        foreach (var mapping in endpointsMapping)
        {
            worksheet.Cell($"A{rowIndex}").Value = mapping.Method.ToString();
            worksheet.Cell($"B{rowIndex}").Value = mapping.Endpoint;
            worksheet.Cell($"C{rowIndex}").Value = mapping.TestsCount;

            rowIndex++;
        }

        worksheet.Columns(1, 3);
    }

    private static void FillCoverageMetrics(this IXLWorksheet worksheet)
    {
        worksheet.Cell("E1").Value = "Total endpoints";
        worksheet.Cell("E2").Value = "Covered count";
        worksheet.Cell("E3").Value = "Uncovered count";
        worksheet.Cell("E4").Value = "Coverage %";

        var rngHeaders = worksheet.Range("E1:E4");
        rngHeaders.Style.Font.Bold = true;
        rngHeaders.Style.Fill.BackgroundColor = XLColor.LightGray;

        var testsCountColumn = "C";
        var lastRow = worksheet.LastRowUsed().RowNumber();

        var firstTestCountCell = $"{testsCountColumn}2";
        var lastTestCountCell = $"{testsCountColumn}{lastRow}";

        worksheet.Cell("F1").FormulaA1 = $"=COUNTIF({firstTestCountCell}:{lastTestCountCell},\"<>\")";
        worksheet.Cell("F2").FormulaA1 = $"=COUNTIF({firstTestCountCell}:{lastTestCountCell},\">0\")";
        worksheet.Cell("F3").FormulaA1 = $"=COUNTIF({firstTestCountCell}:{lastTestCountCell},\"=0\")";
        worksheet.Cell("F4").FormulaA1 = "=ROUND(F2/(F1/100), 1)";

        worksheet.Columns(5, 5);
    }
}
