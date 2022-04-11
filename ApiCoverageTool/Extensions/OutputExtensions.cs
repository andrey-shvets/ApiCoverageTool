using System.Globalization;
using System.IO;
using System.Linq;
using ApiCoverageTool.Models;
using CsvHelper;

namespace ApiCoverageTool.Extensions
{
    public static class OutputExtensions
    {
        public static void ToCsvFile(this MappedApiResult mappedApiResult, string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            if (mappedApiResult is null || !mappedApiResult.EndpointsMapping.Any())
            {
                File.WriteAllText(path, string.Empty);
                return;
            }

            var combinedList = mappedApiResult.EndpointsMapping
                .OrderByDescending(m => m.Value.Any())
                .Select(m => new { Method = $"{m.Key.RestMethod} {m.Key.Path}", TestsCount = m.Value.Count })
                .ToList();

            using var writer = new StreamWriter(path);
            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csvWriter.WriteRecords(combinedList);
        }
    }
}
