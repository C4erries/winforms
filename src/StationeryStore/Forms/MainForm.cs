using Npgsql;
using StationeryStore.Data;

namespace StationeryStore;

public sealed class MainForm : Form
{
    private readonly Db _db;

    public MainForm()
    {
        Text = "Учет товаров магазина канцтоваров";
        Width = 1200;
        Height = 760;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.WhiteSmoke;
        _db = new Db();

        var tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            Padding = new Point(12, 5)
        };
        tabs.TabPages.Add(BuildGroupsTab());
        tabs.TabPages.Add(BuildProductsTab());
        tabs.TabPages.Add(BuildSuppliersTab());
        tabs.TabPages.Add(new InvoiceTabPage(_db));
        tabs.TabPages.Add(BuildPaymentsTab());
        tabs.TabPages.Add(new ReportsTabPage(_db));
        Controls.Add(tabs);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _db.Dispose();
        }
        base.Dispose(disposing);
    }

    private CrudTabPage BuildGroupsTab()
    {
        return new CrudTabPage(
            "Группы товаров",
            _db,
            "SELECT id, name AS \"Название\" FROM product_group ORDER BY name",
            (form, row) => form.AddText("name", "Название", row?["Название"].ToString() ?? ""),
            (form, id) => id == null
                ? ("INSERT INTO product_group(name) VALUES(@name)", new[] { new NpgsqlParameter("name", form.TextValue("name")) })
                : ("UPDATE product_group SET name = @name WHERE id = @id", new[] { new NpgsqlParameter("name", form.TextValue("name")), new NpgsqlParameter("id", id.Value) }),
            "DELETE FROM product_group WHERE id = @id");
    }

    private CrudTabPage BuildProductsTab()
    {
        return new CrudTabPage(
            "Товары",
            _db,
            """
            SELECT p.id, p.group_id, pg.name AS "Группа", p.name AS "Название", p.unit AS "Ед."
            FROM product p
            JOIN product_group pg ON pg.id = p.group_id
            ORDER BY pg.name, p.name
            """,
            (form, row) =>
            {
                var groups = CrudTabPage.Lookup(_db, "SELECT id, name FROM product_group ORDER BY name");
                form.AddLookup("group_id", "Группа", groups, row == null ? groups.FirstOrDefault()?.Id ?? 0 : Convert.ToInt32(row["group_id"]));
                form.AddText("name", "Название", row?["Название"].ToString() ?? "");
                form.AddText("unit", "Ед.", row?["Ед."].ToString() ?? "шт");
            },
            (form, id) => id == null
                ? ("INSERT INTO product(group_id, name, unit) VALUES(@group_id, @name, @unit)", new[] { new NpgsqlParameter("group_id", form.LookupId("group_id")), new NpgsqlParameter("name", form.TextValue("name")), new NpgsqlParameter("unit", form.TextValue("unit")) })
                : ("UPDATE product SET group_id = @group_id, name = @name, unit = @unit WHERE id = @id", new[] { new NpgsqlParameter("group_id", form.LookupId("group_id")), new NpgsqlParameter("name", form.TextValue("name")), new NpgsqlParameter("unit", form.TextValue("unit")), new NpgsqlParameter("id", id.Value) }),
            "DELETE FROM product WHERE id = @id");
    }

    private CrudTabPage BuildSuppliersTab()
    {
        return new CrudTabPage(
            "Поставщики",
            _db,
            "SELECT id, name AS \"Название\", address AS \"Адрес\", phone AS \"Телефон\" FROM supplier ORDER BY name",
            (form, row) =>
            {
                form.AddText("name", "Название", row?["Название"].ToString() ?? "");
                form.AddText("address", "Адрес", row?["Адрес"].ToString() ?? "");
                form.AddText("phone", "Телефон", row?["Телефон"].ToString() ?? "");
            },
            (form, id) => id == null
                ? ("INSERT INTO supplier(name, address, phone) VALUES(@name, @address, @phone)", new[] { new NpgsqlParameter("name", form.TextValue("name")), new NpgsqlParameter("address", form.TextValue("address")), new NpgsqlParameter("phone", form.TextValue("phone")) })
                : ("UPDATE supplier SET name = @name, address = @address, phone = @phone WHERE id = @id", new[] { new NpgsqlParameter("name", form.TextValue("name")), new NpgsqlParameter("address", form.TextValue("address")), new NpgsqlParameter("phone", form.TextValue("phone")), new NpgsqlParameter("id", id.Value) }),
            "DELETE FROM supplier WHERE id = @id");
    }

    private CrudTabPage BuildPaymentsTab()
    {
        return new CrudTabPage(
            "Платежи",
            _db,
            """
            SELECT p.id, p.invoice_id, i.number AS "Накладная", s.name AS "Поставщик",
                   p.payment_date AS "Дата", p.amount AS "Сумма", p.note AS "Примечание"
            FROM payment p
            JOIN invoice i ON i.id = p.invoice_id
            JOIN supplier s ON s.id = i.supplier_id
            ORDER BY p.payment_date DESC, p.id DESC
            """,
            (form, row) =>
            {
                var invoices = CrudTabPage.Lookup(_db, """
                    SELECT i.id, i.number || ' от ' || to_char(i.invoice_date, 'DD.MM.YYYY') || ' - ' || s.name AS name
                    FROM invoice i
                    JOIN supplier s ON s.id = i.supplier_id
                    WHERE i.invoice_type = 'income'
                    ORDER BY i.invoice_date DESC, i.number
                    """);
                form.AddLookup("invoice_id", "Накладная", invoices, row == null ? invoices.FirstOrDefault()?.Id ?? 0 : Convert.ToInt32(row["invoice_id"]));
                form.AddDate("payment_date", "Дата", row == null ? DateTime.Today : Convert.ToDateTime(row["Дата"]));
                form.AddDecimal("amount", "Сумма", row == null ? 0 : Convert.ToDecimal(row["Сумма"]));
                form.AddText("note", "Примечание", row?["Примечание"].ToString() ?? "");
            },
            (form, id) => id == null
                ? ("INSERT INTO payment(invoice_id, payment_date, amount, note) VALUES(@invoice_id, @payment_date, @amount, @note)", new[] { new NpgsqlParameter("invoice_id", form.LookupId("invoice_id")), new NpgsqlParameter("payment_date", form.DateValue("payment_date")), new NpgsqlParameter("amount", form.DecimalValue("amount")), new NpgsqlParameter("note", form.TextValue("note")) })
                : ("UPDATE payment SET invoice_id = @invoice_id, payment_date = @payment_date, amount = @amount, note = @note WHERE id = @id", new[] { new NpgsqlParameter("invoice_id", form.LookupId("invoice_id")), new NpgsqlParameter("payment_date", form.DateValue("payment_date")), new NpgsqlParameter("amount", form.DecimalValue("amount")), new NpgsqlParameter("note", form.TextValue("note")), new NpgsqlParameter("id", id.Value) }),
            "DELETE FROM payment WHERE id = @id",
            ValidatePaymentAmount);
    }

    private bool ValidatePaymentAmount(EditRecordForm form, int? paymentId)
    {
        var invoiceId = form.LookupId("invoice_id");
        var amount = form.DecimalValue("amount");
        var maxAmount = Convert.ToDecimal(_db.Scalar("""
            SELECT COALESCE(i.current_debt, 0) +
                   COALESCE((
                       SELECT p.amount
                       FROM payment p
                       WHERE p.id = @payment_id
                         AND p.invoice_id = i.id
                   ), 0)
            FROM invoice i
            WHERE i.id = @invoice_id
            """,
            new NpgsqlParameter("invoice_id", invoiceId),
            new NpgsqlParameter("payment_id", (object?)paymentId ?? DBNull.Value)) ?? 0);

        if (amount <= maxAmount)
        {
            return true;
        }

        MessageBox.Show(
            $"Сумма платежа не может быть больше задолженности.\nМаксимум можно заплатить: {maxAmount:N2}",
            "Ошибка",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
        return false;
    }
}
