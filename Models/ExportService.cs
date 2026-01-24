using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Text;
using iTextTextAlignment = iText.Layout.Properties.TextAlignment;
using iTextCell = iText.Layout.Element.Cell;

namespace MoodAtlas.Services;

public class ExportService
{
    private readonly EntryService _entryService;
    private readonly MoodService _moodService;
    private readonly TagService _tagService;

    public ExportService()
    {
        _entryService = new EntryService();
        _moodService = new MoodService();
        _tagService = new TagService();
    }

    public async Task<string> ExportToPdfAsync(int userId, DateTime startDate, DateTime endDate, string outputPath = null)
    {
        try
        {
            // Get entries for the date range
            var entries = await _entryService.GetEntriesByDateRangeAsync(userId, startDate, endDate);

            if (entries == null || entries.Count == 0)
                throw new Exception("No entries found for the selected date range");

            // Create output file path
            if (string.IsNullOrEmpty(outputPath))
            {
                var fileName = $"MoodAtlas_Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                
                // Ensure the directory exists
                var appDataDir = FileSystem.AppDataDirectory;
                if (!Directory.Exists(appDataDir))
                {
                    Directory.CreateDirectory(appDataDir);
                }
                
                outputPath = Path.Combine(appDataDir, fileName);
            }

            // Create PDF with error handling
            using (var writer = new PdfWriter(outputPath))
            using (var pdf = new PdfDocument(writer))
            using (var document = new Document(pdf))
            {
                // Add title and header
                AddHeader(document, startDate, endDate, entries.Count);

                // Add summary statistics
                await AddSummary(document, userId, startDate, endDate, entries);

                // Add entries
                await AddEntries(document, entries);
            }

            // Verify file was created
            if (!File.Exists(outputPath))
            {
                throw new Exception("PDF file was not created successfully");
            }

            return outputPath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Export PDF Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw new Exception($"Failed to export PDF: {ex.Message}", ex);
        }
    }

    private void AddHeader(Document document, DateTime startDate, DateTime endDate, int entryCount)
    {
        // Title
        document.Add(new Paragraph("MoodAtlas Journal Export")
            .SetTextAlignment(iTextTextAlignment.CENTER)
            .SetFontSize(24)
            .SetBold()
            .SetMarginBottom(10));

        // Subtitle
        document.Add(new Paragraph($"Period: {startDate:MMMM dd, yyyy} to {endDate:MMMM dd, yyyy}")
            .SetTextAlignment(iTextTextAlignment.CENTER)
            .SetFontSize(14)
            .SetFontColor(ColorConstants.GRAY)
            .SetMarginBottom(20));

        // Export info
        document.Add(new Paragraph($"Total Entries: {entryCount}")
            .SetTextAlignment(iTextTextAlignment.CENTER)
            .SetFontSize(12)
            .SetItalic()
            .SetMarginBottom(30));

        document.Add(new LineSeparator(new SolidLine())
            .SetMarginBottom(30));
    }

    private async Task AddSummary(Document document, int userId, DateTime startDate, DateTime endDate, List<Models.Entry> entries)
    {
        var analyticsService = new AnalyticsService();
        var analytics = await analyticsService.GetAnalyticsAsync(userId, startDate, endDate);

        // Create summary table
        Table summaryTable = new Table(2, false)
            .SetWidth(UnitValue.CreatePercentValue(100))
            .SetMarginBottom(30);

        summaryTable.AddHeaderCell(new iTextCell().Add(new Paragraph("Metric").SetBold()));
        summaryTable.AddHeaderCell(new iTextCell().Add(new Paragraph("Value").SetBold()));

        summaryTable.AddCell("Current Streak");
        summaryTable.AddCell($"{analytics.CurrentStreak} days");

        summaryTable.AddCell("Longest Streak");
        summaryTable.AddCell($"{analytics.LongestStreak} days");

        summaryTable.AddCell("Missed Days");
        summaryTable.AddCell($"{analytics.MissedDays} days");

        summaryTable.AddCell("Average Word Count");
        summaryTable.AddCell($"{analytics.AverageWordCount} words");

        summaryTable.AddCell("Most Frequent Mood");
        summaryTable.AddCell(analytics.MostFrequentMood);

        document.Add(new Paragraph("Summary Statistics")
            .SetFontSize(18)
            .SetBold()
            .SetMarginBottom(15));

        document.Add(summaryTable);
    }

    private async Task AddEntries(Document document, List<Models.Entry> entries)
    {
        document.Add(new Paragraph("Journal Entries")
            .SetFontSize(18)
            .SetBold()
            .SetMarginBottom(20));

        foreach (var entry in entries.OrderByDescending(e => e.EntryDate))
        {
            // Entry header
            document.Add(new Paragraph(entry.EntryDate.ToString("dddd, MMMM dd, yyyy"))
                .SetFontSize(16)
                .SetBold()
                .SetMarginTop(20));

            // Get moods and tags for this entry
            var moods = await _moodService.GetMoodsForEntryAsync(entry.EntryId);
            var tags = await _tagService.GetTagsForEntryAsync(entry.EntryId);

            // Add moods if available
            if (moods.Any())
            {
                var moodText = "Moods: " + string.Join(", ", moods.Select(m => $"{m.Emoji} {m.Name}"));
                document.Add(new Paragraph(moodText)
                    .SetFontSize(12)
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetMarginBottom(5));
            }

            // Add tags if available
            if (tags.Any())
            {
                var tagText = "Tags: " + string.Join(", ", tags.Select(t => t.Name));
                document.Add(new Paragraph(tagText)
                    .SetFontSize(11)
                    .SetFontColor(ColorConstants.GRAY)
                    .SetMarginBottom(10));
            }

            // Entry title
            if (!string.IsNullOrEmpty(entry.Title))
            {
                document.Add(new Paragraph(entry.Title)
                    .SetFontSize(14)
                    .SetBold()
                    .SetMarginBottom(10));
            }

            // Entry content
            document.Add(new Paragraph(entry.Content)
                .SetFontSize(11)
                .SetMarginBottom(10));

            // Word count
            document.Add(new Paragraph($"Word count: {entry.WordCount}")
                .SetFontSize(10)
                .SetItalic()
                .SetFontColor(ColorConstants.GRAY)
                .SetMarginBottom(20));

            // Separator
            if (entry != entries.Last())
            {
                document.Add(new LineSeparator(new DottedLine())
                    .SetMarginBottom(20));
            }
        }
    }

    public async Task<string> ExportToTextAsync(int userId, DateTime startDate, DateTime endDate)
    {
        var entries = await _entryService.GetEntriesByDateRangeAsync(userId, startDate, endDate);

        var sb = new StringBuilder();

        sb.AppendLine("=== MoodAtlas Journal Export ===");
        sb.AppendLine($"Period: {startDate:MMMM dd, yyyy} to {endDate:MMMM dd, yyyy}");
        sb.AppendLine($"Total Entries: {entries.Count}");
        sb.AppendLine();
        sb.AppendLine("=================================");
        sb.AppendLine();

        foreach (var entry in entries.OrderByDescending(e => e.EntryDate))
        {
            sb.AppendLine($"Date: {entry.EntryDate:dddd, MMMM dd, yyyy}");
            sb.AppendLine($"Title: {entry.Title}");

            var moods = await _moodService.GetMoodsForEntryAsync(entry.EntryId);
            if (moods.Any())
            {
                sb.AppendLine($"Moods: {string.Join(", ", moods.Select(m => $"{m.Emoji} {m.Name}"))}");
            }

            var tags = await _tagService.GetTagsForEntryAsync(entry.EntryId);
            if (tags.Any())
            {
                sb.AppendLine($"Tags: {string.Join(", ", tags.Select(t => t.Name))}");
            }

            sb.AppendLine();
            sb.AppendLine(entry.Content);
            sb.AppendLine();
            sb.AppendLine($"Word count: {entry.WordCount}");
            sb.AppendLine("---");
            sb.AppendLine();
        }

        return sb.ToString();
    }
}