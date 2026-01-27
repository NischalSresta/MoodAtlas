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
                throw new Exception("No entries found for the selected date range. Please create some journal entries first.");

            // Create output file path
            if (string.IsNullOrEmpty(outputPath))
            {
                var fileName = $"MoodAtlas_Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var cacheDir = FileSystem.CacheDirectory;
                
                if (!Directory.Exists(cacheDir))
                {
                    Directory.CreateDirectory(cacheDir);
                }
                
                outputPath = Path.Combine(cacheDir, fileName);
            }

            // Delete existing file if it exists
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            // Create PDF with proper initialization
            var writerProperties = new WriterProperties();
            
            using (var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var writer = new PdfWriter(stream, writerProperties))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        using (var document = new Document(pdf))
                        {
                            // Add title and header
                            AddHeader(document, startDate, endDate, entries.Count);

                            // Add summary statistics
                            await AddSummary(document, userId, startDate, endDate, entries);

                            // Add entries
                            await AddEntries(document, entries);
                        }
                    }
                }
            }

            // Verify file was created
            if (!File.Exists(outputPath))
            {
                throw new Exception("PDF file was not created successfully");
            }

            return outputPath;
        }
        catch (iText.Kernel.Exceptions.PdfException pdfEx)
        {
            System.Diagnostics.Debug.WriteLine($"iText PDF Error: {pdfEx.Message}");
            // Fall back to text export
            return await ExportToTextFileAsync(userId, startDate, endDate);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Export PDF Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw new Exception($"{ex.Message}", ex);
        }
    }

    /// <summary>
    /// Fallback: Export to text file when PDF fails
    /// </summary>
    public async Task<string> ExportToTextFileAsync(int userId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var content = await ExportToTextAsync(userId, startDate, endDate);
            
            var fileName = $"MoodAtlas_Export_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var cacheDir = FileSystem.CacheDirectory;
            
            if (!Directory.Exists(cacheDir))
            {
                Directory.CreateDirectory(cacheDir);
            }
            
            var filePath = Path.Combine(cacheDir, fileName);
            await File.WriteAllTextAsync(filePath, content);
            
            return filePath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Export Text Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Exports and opens the native share dialog
    /// </summary>
    public async Task<bool> ExportAndSharePdfAsync(int userId, DateTime startDate, DateTime endDate)
    {
        try
        {
            // Try PDF first, fallback to text
            string filePath;
            try
            {
                filePath = await ExportToPdfAsync(userId, startDate, endDate);
            }
            catch (Exception pdfEx)
            {
                System.Diagnostics.Debug.WriteLine($"PDF failed, trying text: {pdfEx.Message}");
                filePath = await ExportToTextFileAsync(userId, startDate, endDate);
            }

            // Use MAUI's Share API
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Export MoodAtlas Journal",
                File = new ShareFile(filePath)
            });

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Share Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Exports and saves to Documents folder
    /// </summary>
    public async Task<string> ExportAndSavePdfAsync(int userId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var tempFilePath = await ExportToPdfAsync(userId, startDate, endDate);
            var fileName = Path.GetFileName(tempFilePath);
            string finalPath = tempFilePath;

#if WINDOWS
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var moodAtlasFolder = Path.Combine(documentsPath, "MoodAtlas");
                
                if (!Directory.Exists(moodAtlasFolder))
                {
                    Directory.CreateDirectory(moodAtlasFolder);
                }
                
                finalPath = Path.Combine(moodAtlasFolder, fileName);
                File.Copy(tempFilePath, finalPath, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Could not save to Documents: {ex.Message}");
            }
#endif

            return finalPath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save PDF Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Opens the exported file using the default viewer
    /// </summary>
    public async Task<bool> OpenFileAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Open File Error: {ex.Message}");
            throw;
        }
    }

    private void AddHeader(Document document, DateTime startDate, DateTime endDate, int entryCount)
    {
        document.Add(new Paragraph("MoodAtlas Journal Export")
            .SetTextAlignment(iTextTextAlignment.CENTER)
            .SetFontSize(24)
            .SetBold()
            .SetMarginBottom(10));

        document.Add(new Paragraph($"Period: {startDate:MMMM dd, yyyy} to {endDate:MMMM dd, yyyy}")
            .SetTextAlignment(iTextTextAlignment.CENTER)
            .SetFontSize(14)
            .SetFontColor(ColorConstants.GRAY)
            .SetMarginBottom(20));

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
        try
        {
            var analyticsService = new AnalyticsService();
            var analytics = await analyticsService.GetAnalyticsAsync(userId, startDate, endDate);

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
            summaryTable.AddCell(analytics.MostFrequentMood ?? "N/A");

            document.Add(new Paragraph("Summary Statistics")
                .SetFontSize(18)
                .SetBold()
                .SetMarginBottom(15));

            document.Add(summaryTable);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error adding summary: {ex.Message}");
            // Continue without summary
        }
    }

    private async Task AddEntries(Document document, List<Models.Entry> entries)
    {
        document.Add(new Paragraph("Journal Entries")
            .SetFontSize(18)
            .SetBold()
            .SetMarginBottom(20));

        foreach (var entry in entries.OrderByDescending(e => e.EntryDate))
        {
            try
            {
                document.Add(new Paragraph(entry.EntryDate.ToString("dddd, MMMM dd, yyyy"))
                    .SetFontSize(16)
                    .SetBold()
                    .SetMarginTop(20));

                var moods = await _moodService.GetMoodsForEntryAsync(entry.EntryId);
                var tags = await _tagService.GetTagsForEntryAsync(entry.EntryId);

                if (moods != null && moods.Any())
                {
                    var moodText = "Moods: " + string.Join(", ", moods.Select(m => $"{m.Emoji} {m.Name}"));
                    document.Add(new Paragraph(moodText)
                        .SetFontSize(12)
                        .SetFontColor(ColorConstants.DARK_GRAY)
                        .SetMarginBottom(5));
                }

                if (tags != null && tags.Any())
                {
                    var tagText = "Tags: " + string.Join(", ", tags.Select(t => t.Name));
                    document.Add(new Paragraph(tagText)
                        .SetFontSize(11)
                        .SetFontColor(ColorConstants.GRAY)
                        .SetMarginBottom(10));
                }

                if (!string.IsNullOrEmpty(entry.Title))
                {
                    document.Add(new Paragraph(entry.Title)
                        .SetFontSize(14)
                        .SetBold()
                        .SetMarginBottom(10));
                }

                if (!string.IsNullOrEmpty(entry.Content))
                {
                    document.Add(new Paragraph(entry.Content)
                        .SetFontSize(11)
                        .SetMarginBottom(10));
                }

                document.Add(new Paragraph($"Word count: {entry.WordCount}")
                    .SetFontSize(10)
                    .SetItalic()
                    .SetFontColor(ColorConstants.GRAY)
                    .SetMarginBottom(20));

                if (entry != entries.Last())
                {
                    document.Add(new LineSeparator(new DottedLine())
                        .SetMarginBottom(20));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding entry {entry.EntryId}: {ex.Message}");
            }
        }
    }

    public async Task<string> ExportToTextAsync(int userId, DateTime startDate, DateTime endDate)
    {
        var entries = await _entryService.GetEntriesByDateRangeAsync(userId, startDate, endDate);

        if (entries == null || entries.Count == 0)
            throw new Exception("No entries found for the selected date range");

        var sb = new StringBuilder();

        sb.AppendLine("╔══════════════════════════════════════════════════════════════╗");
        sb.AppendLine("║                   MoodAtlas Journal Export                    ║");
        sb.AppendLine("╚══════════════════════════════════════════════════════════════╝");
        sb.AppendLine();
        sb.AppendLine($"📅 Period: {startDate:MMMM dd, yyyy} to {endDate:MMMM dd, yyyy}");
        sb.AppendLine($"📝 Total Entries: {entries.Count}");
        sb.AppendLine($"🕐 Exported: {DateTime.Now:MMMM dd, yyyy HH:mm}");
        sb.AppendLine();
        sb.AppendLine("══════════════════════════════════════════════════════════════");
        sb.AppendLine();

        foreach (var entry in entries.OrderByDescending(e => e.EntryDate))
        {
            sb.AppendLine($"📆 {entry.EntryDate:dddd, MMMM dd, yyyy}");
            sb.AppendLine($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            
            if (!string.IsNullOrEmpty(entry.Title))
            {
                sb.AppendLine($"📌 Title: {entry.Title}");
            }

            var moods = await _moodService.GetMoodsForEntryAsync(entry.EntryId);
            if (moods != null && moods.Any())
            {
                sb.AppendLine($"😊 Moods: {string.Join(", ", moods.Select(m => $"{m.Emoji} {m.Name}"))}");
            }

            var tags = await _tagService.GetTagsForEntryAsync(entry.EntryId);
            if (tags != null && tags.Any())
            {
                sb.AppendLine($"🏷️ Tags: {string.Join(", ", tags.Select(t => t.Name))}");
            }

            sb.AppendLine();
            if (!string.IsNullOrEmpty(entry.Content))
            {
                sb.AppendLine(entry.Content);
            }
            sb.AppendLine();
            sb.AppendLine($"📊 Word count: {entry.WordCount}");
            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────────────────────────────");
            sb.AppendLine();
        }

        sb.AppendLine();
        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine("                    Thank you for using MoodAtlas!              ");
        sb.AppendLine("═══════════════════════════════════════════════════════════════");

        return sb.ToString();
    }
}