using System.Data;
using Npgsql;
using StationeryStore.Data;
using StationeryStore.Models;
using StationeryStore.Services;

namespace StationeryStore;

public sealed class InvoiceTabPage : TabPage
{
    private readonly Db _db;
    private readonly DataGridView _invoiceGrid = new() { Dock = DockStyle.Fill };
    private readonly DataGridView _itemGrid = new() { Dock = DockStyle.Fill };

    public InvoiceTabPage(Db db)
    {
        _db = db;
        Text = "Накладные";
        UiStyles.ApplyPage(this);
        UiStyles.ApplyGrid(_invoiceGrid);
        UiStyles.ApplyGrid(_itemGrid);

        var invoiceButtons = BuildButtons(("Добавить", AddInvoice), ("Изменить", EditInvoice), ("Удалить", DeleteInvoice), ("Экспорт Word", ExportWord), ("Обновить", LoadInvoices));
        var itemButtons = BuildButtons(("Добавить товар", AddItem), ("Изменить товар", EditItem), ("Удалить товар", DeleteItem));

        var invoicePanel = new Panel { Dock = DockStyle.Top, Height = 315, Padding = new Padding(0, 0, 0, 8) };
        invoicePanel.Controls.Add(_invoiceGrid);
        invoicePanel.Controls.Add(invoiceButtons);
        invoicePanel.Controls.Add(UiStyles.SectionTitle("Накладные"));

        var itemPanel = new Panel { Dock = DockStyle.Fill };
        itemPanel.Controls.Add(_itemGrid);
        itemPanel.Controls.Add(itemButtons);
        itemPanel.Controls.Add(UiStyles.SectionTitle("Состав выбранной накладной"));

        Controls.Add(itemPanel);
        Controls.Add(invoicePanel);

        _invoiceGrid.SelectionChanged += (_, _) => LoadItems();
        LoadInvoices();
    }

    public void LoadInvoices()
    {
        _invoiceGrid.DataSource = _db.Query("""
            SELECT i.id, i.number AS "Номер", i.invoice_date AS "Дата",
                   CASE WHEN i.invoice_type = 'income' THEN 'Приходная' ELSE 'Расходная' END AS "Тип",
                   i.invoice_type, i.supplier_id, s.name AS "Поставщик",
                   i.note AS "Примечание", i.total_sum AS "Сумма"
            FROM invoice i
            JOIN supplier s ON s.id = i.supplier_id
            ORDER BY i.invoice_date DESC, i.id DESC
            """);
        HideInvoiceTechnicalColumns();
        LoadItems();
    }

    private void LoadItems()
    {
        var invoiceId = SelectedInvoiceId();
        if (invoiceId == 0)
        {
            _itemGrid.DataSource = null;
            return;
        }

        _itemGrid.DataSource = _db.Query("""
            SELECT ii.id, ii.product_id, p.name AS "Товар", pg.name AS "Группа",
                   p.unit AS "Ед.", ii.quantity AS "Количество", ii.price AS "Цена",
                   ii.quantity * ii.price AS "Сумма"
            FROM invoice_item ii
            JOIN product p ON p.id = ii.product_id
            JOIN product_group pg ON pg.id = p.group_id
            WHERE ii.invoice_id = @id
            ORDER BY ii.id
            """, new NpgsqlParameter("id", invoiceId));
        UiStyles.HideTechnicalColumns(_itemGrid, "product_id");
        _itemGrid.ClearSelection();
    }

    private static FlowLayoutPanel BuildButtons(params (string Text, Action Handler)[] buttons)
    {
        var panel = UiStyles.ButtonPanel();
        foreach (var (text, handler) in buttons)
        {
            var button = UiStyles.Button(text, text.Length > 12 ? 150 : 124);
            button.Click += (_, _) => handler();
            panel.Controls.Add(button);
        }
        return panel;
    }

    private void AddInvoice() => OpenInvoiceEditor(null);
    private void EditInvoice()
    {
        var row = CurrentInvoiceRow();
        if (row != null) OpenInvoiceEditor(row);
    }

    private void OpenInvoiceEditor(DataRow? row)
    {
        var suppliers = CrudTabPage.Lookup(_db, "SELECT id, name FROM supplier ORDER BY name");
        if (suppliers.Count == 0)
        {
            MessageBox.Show("Сначала добавьте поставщика.");
            return;
        }

        var form = new EditRecordForm(row == null ? "Добавление накладной" : "Изменение накладной");
        form.AddText("number", "Номер", row?["Номер"].ToString() ?? "");
        form.AddLookup("supplier_id", "Поставщик", suppliers, row == null ? suppliers[0].Id : Convert.ToInt32(row["supplier_id"]));
        form.AddDate("invoice_date", "Дата", row == null ? DateTime.Today : Convert.ToDateTime(row["Дата"]));
        form.AddLookup("invoice_type", "Тип", new[] { new LookupItem(1, "Приходная"), new LookupItem(2, "Расходная") }, row?["invoice_type"].ToString() == "expense" ? 2 : 1);
        form.AddText("note", "Примечание", row?["Примечание"].ToString() ?? "");
        form.BuildButtons();

        if (form.ShowDialog(this) != DialogResult.OK) return;

        var type = form.LookupId("invoice_type") == 1 ? "income" : "expense";
        if (row == null)
        {
            _db.Execute("""
                INSERT INTO invoice(number, supplier_id, invoice_date, invoice_type, note)
                VALUES(@number, @supplier_id, @invoice_date, @invoice_type, @note)
                """,
                new NpgsqlParameter("number", form.TextValue("number")),
                new NpgsqlParameter("supplier_id", form.LookupId("supplier_id")),
                new NpgsqlParameter("invoice_date", form.DateValue("invoice_date")),
                new NpgsqlParameter("invoice_type", type),
                new NpgsqlParameter("note", form.TextValue("note")));
        }
        else
        {
            _db.Execute("""
                UPDATE invoice
                SET number = @number, supplier_id = @supplier_id, invoice_date = @invoice_date,
                    invoice_type = @invoice_type, note = @note
                WHERE id = @id
                """,
                new NpgsqlParameter("number", form.TextValue("number")),
                new NpgsqlParameter("supplier_id", form.LookupId("supplier_id")),
                new NpgsqlParameter("invoice_date", form.DateValue("invoice_date")),
                new NpgsqlParameter("invoice_type", type),
                new NpgsqlParameter("note", form.TextValue("note")),
                new NpgsqlParameter("id", Convert.ToInt32(row["id"])));
        }
        LoadInvoices();
    }

    private void DeleteInvoice()
    {
        var row = CurrentInvoiceRow();
        if (row == null) return;
        if (MessageBox.Show("Удалить выбранную накладную?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        _db.Execute("DELETE FROM invoice WHERE id = @id", new NpgsqlParameter("id", Convert.ToInt32(row["id"])));
        LoadInvoices();
    }

    private void AddItem() => OpenItemEditor(null);
    private void EditItem()
    {
        var row = CurrentItemRow();
        if (row != null) OpenItemEditor(row);
    }

    private void OpenItemEditor(DataRow? row)
    {
        var invoiceId = SelectedInvoiceId();
        if (invoiceId == 0)
        {
            MessageBox.Show("Выберите накладную.");
            return;
        }

        var products = CrudTabPage.Lookup(_db, """
            SELECT p.id, pg.name || ' - ' || p.name AS name
            FROM product p
            JOIN product_group pg ON pg.id = p.group_id
            ORDER BY pg.name, p.name
            """);
        if (products.Count == 0)
        {
            MessageBox.Show("Сначала добавьте товары.");
            return;
        }

        var form = new EditRecordForm(row == null ? "Добавление товара" : "Изменение товара");
        form.AddLookup("product_id", "Товар", products, row == null ? products[0].Id : Convert.ToInt32(row["product_id"]));
        form.AddDecimal("quantity", "Количество", row == null ? 1 : Convert.ToDecimal(row["Количество"]));
        form.AddDecimal("price", "Цена", row == null ? 0 : Convert.ToDecimal(row["Цена"]));
        form.BuildButtons();
        if (form.ShowDialog(this) != DialogResult.OK) return;

        if (row == null)
        {
            _db.Execute("""
                INSERT INTO invoice_item(invoice_id, product_id, quantity, price)
                VALUES(@invoice_id, @product_id, @quantity, @price)
                """,
                new NpgsqlParameter("invoice_id", invoiceId),
                new NpgsqlParameter("product_id", form.LookupId("product_id")),
                new NpgsqlParameter("quantity", form.DecimalValue("quantity")),
                new NpgsqlParameter("price", form.DecimalValue("price")));
        }
        else
        {
            _db.Execute("""
                UPDATE invoice_item
                SET product_id = @product_id, quantity = @quantity, price = @price
                WHERE id = @id
                """,
                new NpgsqlParameter("product_id", form.LookupId("product_id")),
                new NpgsqlParameter("quantity", form.DecimalValue("quantity")),
                new NpgsqlParameter("price", form.DecimalValue("price")),
                new NpgsqlParameter("id", Convert.ToInt32(row["id"])));
        }
        LoadInvoices();
    }

    private void DeleteItem()
    {
        var row = CurrentItemRow();
        if (row == null) return;
        if (MessageBox.Show("Удалить выбранный товар?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        _db.Execute("DELETE FROM invoice_item WHERE id = @id", new NpgsqlParameter("id", Convert.ToInt32(row["id"])));
        LoadInvoices();
    }

    private void ExportWord()
    {
        var invoiceId = SelectedInvoiceId();
        if (invoiceId == 0)
        {
            MessageBox.Show("Выберите накладную.");
            return;
        }

        using var dialog = new SaveFileDialog { Filter = "Word document (*.docx)|*.docx", FileName = "nakladnaya.docx" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        ExportService.ExportInvoiceToWord(_db, invoiceId, dialog.FileName);
        MessageBox.Show("Накладная экспортирована в Word.");
    }

    private int SelectedInvoiceId() => CurrentInvoiceRow(false) is { } row ? Convert.ToInt32(row["id"]) : 0;

    private DataRow? CurrentInvoiceRow(bool showMessage = true)
    {
        if (_invoiceGrid.CurrentRow?.DataBoundItem is DataRowView view) return view.Row;
        if (showMessage) MessageBox.Show("Выберите накладную.");
        return null;
    }

    private DataRow? CurrentItemRow()
    {
        if (_itemGrid.CurrentRow?.DataBoundItem is DataRowView view) return view.Row;
        MessageBox.Show("Выберите товар в накладной.");
        return null;
    }

    private void HideInvoiceTechnicalColumns()
    {
        UiStyles.HideTechnicalColumns(_invoiceGrid, "invoice_type", "supplier_id");
        _invoiceGrid.ClearSelection();
    }
}
