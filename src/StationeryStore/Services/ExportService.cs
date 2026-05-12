using System.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using StationeryStore.Data;

namespace StationeryStore.Services;

public static class ExportService
{
    public static void ExportTableToExcel(DataTable table, string fileName)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Отчет");
        sheet.Cell(1, 1).InsertTable(table, "Report", true);
        sheet.Columns().AdjustToContents();
        workbook.SaveAs(fileName);
    }

    public static void ExportInvoiceToWord(Db db, int invoiceId, string fileName)
    {
        var invoice = db.Query("""
            SELECT i.number, i.invoice_date::timestamp AS invoice_date, i.invoice_type, i.total_sum,
                   s.name AS supplier_name, s.address, s.phone
            FROM invoice i
            JOIN supplier s ON s.id = i.supplier_id
            WHERE i.id = @id
            """, new Npgsql.NpgsqlParameter("id", invoiceId));

        if (invoice.Rows.Count == 0)
        {
            throw new InvalidOperationException("Накладная не найдена.");
        }

        var row = invoice.Rows[0];
        var items = db.Query("""
            SELECT p.name AS product_name, p.unit, ii.quantity, ii.price,
                   ii.quantity * ii.price AS sum
            FROM invoice_item ii
            JOIN product p ON p.id = ii.product_id
            WHERE ii.invoice_id = @id
            ORDER BY ii.id
            """, new Npgsql.NpgsqlParameter("id", invoiceId));

        using var document = WordprocessingDocument.Create(fileName, WordprocessingDocumentType.Document);
        var main = document.AddMainDocumentPart();
        main.Document = new Document(new Body());
        var body = main.Document.Body!;

        var type = row["invoice_type"].ToString() == "income" ? "Приходная" : "Расходная";
        AddParagraph(body, $"{type} накладная № {row["number"]}", true);
        AddParagraph(body, $"Дата: {Convert.ToDateTime(row["invoice_date"]):dd.MM.yyyy}");
        AddParagraph(body, $"Поставщик: {row["supplier_name"]}");
        AddParagraph(body, $"Адрес: {row["address"]}");
        AddParagraph(body, $"Телефон: {row["phone"]}");
        AddParagraph(body, "");

        var table = new Table();
        table.AppendChild(new TableProperties(new TableBorders(
            new TopBorder { Val = BorderValues.Single, Size = 4 },
            new BottomBorder { Val = BorderValues.Single, Size = 4 },
            new LeftBorder { Val = BorderValues.Single, Size = 4 },
            new RightBorder { Val = BorderValues.Single, Size = 4 },
            new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
            new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 })));

        AddRow(table, "Товар", "Ед.", "Количество", "Цена", "Сумма");
        foreach (DataRow item in items.Rows)
        {
            AddRow(table,
                item["product_name"].ToString() ?? "",
                item["unit"].ToString() ?? "",
                Convert.ToDecimal(item["quantity"]).ToString("0.###"),
                Convert.ToDecimal(item["price"]).ToString("0.00"),
                Convert.ToDecimal(item["sum"]).ToString("0.00"));
        }

        body.Append(table);
        AddParagraph(body, $"Итого: {Convert.ToDecimal(row["total_sum"]):0.00}");
        main.Document.Save();
    }

    private static void AddParagraph(Body body, string text, bool bold = false)
    {
        var run = new Run(new Text(text));
        if (bold)
        {
            run.RunProperties = new RunProperties(new Bold());
        }
        body.Append(new Paragraph(run));
    }

    private static void AddRow(Table table, params string[] values)
    {
        var row = new TableRow();
        foreach (var value in values)
        {
            row.Append(new TableCell(new Paragraph(new Run(new Text(value)))));
        }
        table.Append(row);
    }
}
