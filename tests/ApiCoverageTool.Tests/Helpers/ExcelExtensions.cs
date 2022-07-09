using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClosedXML.Excel;

namespace ApiCoverageTool.Tests.Helpers;

internal static class ExcelExtensions
{
    public static string[][] ToStringArray(this IXLRange range)
    {
        var table = new List<string[]>();

        foreach (var row in range.Rows())
        {
            var rowValues = row.Cells(usedCellsOnly: true).Select(cell => cell.Value.ToString()).ToArray();

            if (!rowValues.All(string.IsNullOrWhiteSpace))
                table.Add(rowValues);
        }

        return table.ToArray();
    }

    public static string ToCsvString(this IXLRange range)
    {
        var stringArray = range.ToStringArray();
        var builder = new StringBuilder();

        foreach (var row in stringArray)
            builder.AppendLine(string.Join(',', row));

        return builder.ToString();
    }
}
