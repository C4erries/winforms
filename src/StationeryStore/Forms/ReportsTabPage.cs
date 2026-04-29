using System.Data;
using System.Drawing.Drawing2D;
using Npgsql;
using StationeryStore.Data;
using StationeryStore.Services;

namespace StationeryStore;

public sealed class ReportsTabPage : TabPage
{
    private readonly Db _db;
    private readonly CheckedListBox _groups = new() { Dock = DockStyle.Fill, CheckOnClick = true, BorderStyle = BorderStyle.FixedSingle };
    private readonly DateTimePicker _from = new() { Format = DateTimePickerFormat.Short, Width = 120 };
    private readonly DateTimePicker _to = new() { Format = DateTimePickerFormat.Short, Width = 120 };
    private readonly DataGridView _movementGrid = new() { Dock = DockStyle.Fill };
    private readonly DataGridView _debtGrid = new() { Dock = DockStyle.Bottom, Height = 190 };
    private readonly Panel _chart = new() { Dock = DockStyle.Right, Width = 380, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
    private DataTable _lastDebt = new();

    public ReportsTabPage(Db db)
    {
        _db = db;
        Text = "Отчеты";
        UiStyles.ApplyPage(this);
        UiStyles.ApplyGrid(_movementGrid);
        UiStyles.ApplyGrid(_debtGrid);
        _from.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        _to.Value = DateTime.Today;

        var top = UiStyles.ButtonPanel();
        var movement = UiStyles.Button("Движение товаров", 160);
        var excel = UiStyles.Button("Экспорт Excel", 135);
        var debt = UiStyles.Button("Задолженность", 140);
        top.Controls.AddRange(new Control[] { new Label { Text = "С", Width = 20, Height = 32, TextAlign = ContentAlignment.MiddleLeft }, _from, new Label { Text = "По", Width = 25, Height = 32, TextAlign = ContentAlignment.MiddleLeft }, _to, movement, excel, debt });

        movement.Click += (_, _) => BuildMovement();
        excel.Click += (_, _) => ExportMovement();
        debt.Click += (_, _) => BuildDebt();
        _chart.Paint += PaintChart;

        var groupsPanel = new Panel { Dock = DockStyle.Left, Width = 270, Padding = new Padding(0, 0, 10, 0) };
        groupsPanel.Controls.Add(_groups);
        groupsPanel.Controls.Add(UiStyles.SectionTitle("Группы товаров"));

        var reportPanel = new Panel { Dock = DockStyle.Fill };
        reportPanel.Controls.Add(_movementGrid);
        reportPanel.Controls.Add(_debtGrid);
        reportPanel.Controls.Add(UiStyles.SectionTitle("Движение товаров"));

        Controls.Add(reportPanel);
        Controls.Add(_chart);
        Controls.Add(groupsPanel);
        Controls.Add(top);
        LoadGroups();
    }

    private void LoadGroups()
    {
        _groups.Items.Clear();
        foreach (DataRow row in _db.Query("SELECT id, name FROM product_group ORDER BY name").Rows)
        {
            _groups.Items.Add(new GroupItem(Convert.ToInt32(row["id"]), row["name"].ToString() ?? ""), true);
        }
    }

    private void BuildMovement()
    {
        var ids = SelectedGroupIds();
        if (ids.Count == 0)
        {
            MessageBox.Show("Выберите хотя бы одну группу товаров.");
            return;
        }

        _movementGrid.DataSource = _db.Query("""
            SELECT pg.name AS "Группа", p.name AS "Товар", p.unit AS "Ед.",
                   COALESCE(SUM(CASE WHEN i.invoice_date < @from AND i.invoice_type = 'income' THEN ii.quantity
                                      WHEN i.invoice_date < @from AND i.invoice_type = 'expense' THEN -ii.quantity
                                      ELSE 0 END), 0) AS "Остаток на начало",
                   COALESCE(SUM(CASE WHEN i.invoice_date BETWEEN @from AND @to AND i.invoice_type = 'income' THEN ii.quantity ELSE 0 END), 0) AS "Приход",
                   COALESCE(SUM(CASE WHEN i.invoice_date BETWEEN @from AND @to AND i.invoice_type = 'expense' THEN ii.quantity ELSE 0 END), 0) AS "Расход",
                   COALESCE(SUM(CASE WHEN i.invoice_date <= @to AND i.invoice_type = 'income' THEN ii.quantity
                                      WHEN i.invoice_date <= @to AND i.invoice_type = 'expense' THEN -ii.quantity
                                      ELSE 0 END), 0) AS "Остаток на конец"
            FROM product p
            JOIN product_group pg ON pg.id = p.group_id
            LEFT JOIN invoice_item ii ON ii.product_id = p.id
            LEFT JOIN invoice i ON i.id = ii.invoice_id
            WHERE p.group_id = ANY(@groups)
            GROUP BY pg.name, p.name, p.unit
            ORDER BY pg.name, p.name
            """,
            new NpgsqlParameter("from", _from.Value.Date),
            new NpgsqlParameter("to", _to.Value.Date),
            new NpgsqlParameter<int[]>("groups", ids.ToArray()));
    }

    private void BuildDebt()
    {
        _lastDebt = _db.Query("""
            SELECT d::date AS "Дата",
                   GREATEST(
                       COALESCE((
                           SELECT SUM(i.total_sum)
                           FROM invoice i
                           WHERE i.invoice_type = 'income' AND i.invoice_date <= d
                       ), 0)
                       - COALESCE((
                           SELECT SUM(p.amount)
                           FROM payment p
                           JOIN invoice i ON i.id = p.invoice_id
                           WHERE i.invoice_type = 'income' AND p.payment_date <= d
                       ), 0),
                       0
                   ) AS "Задолженность"
            FROM generate_series(@from::date, @to::date, interval '1 day') d
            ORDER BY d
            """,
            new NpgsqlParameter("from", _from.Value.Date),
            new NpgsqlParameter("to", _to.Value.Date));
        _debtGrid.DataSource = _lastDebt;
        _chart.Invalidate();
    }

    private void ExportMovement()
    {
        if (_movementGrid.DataSource is not DataTable table || table.Rows.Count == 0)
        {
            BuildMovement();
            table = _movementGrid.DataSource as DataTable ?? new DataTable();
        }

        using var dialog = new SaveFileDialog { Filter = "Excel workbook (*.xlsx)|*.xlsx", FileName = "dvizhenie-tovarov.xlsx" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        ExportService.ExportTableToExcel(table, dialog.FileName);
        MessageBox.Show("Отчет экспортирован в Excel.");
    }

    private List<int> SelectedGroupIds()
    {
        return _groups.CheckedItems.Cast<GroupItem>().Select(x => x.Id).ToList();
    }

    private void PaintChart(object? sender, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.Clear(Color.White);
        if (_lastDebt.Rows.Count == 0)
        {
            e.Graphics.DrawString("Диаграмма задолженности", Font, Brushes.Black, 20, 20);
            return;
        }

        var values = _lastDebt.Rows.Cast<DataRow>()
            .Select(row => Math.Max(0, Convert.ToDecimal(row["Задолженность"])))
            .ToList();
        var total = values.Sum();
        if (total <= 0)
        {
            e.Graphics.DrawString("Нет задолженности за период", Font, Brushes.Black, 20, 20);
            return;
        }

        var rect = new Rectangle(90, 45, 200, 200);
        var colors = new[] { Color.SteelBlue, Color.Orange, Color.ForestGreen, Color.IndianRed, Color.MediumPurple, Color.Goldenrod };
        decimal start = 0;
        for (var i = 0; i < values.Count; i++)
        {
            var sweep = values[i] / total * 360;
            using var brush = new SolidBrush(colors[i % colors.Length]);
            e.Graphics.FillPie(brush, rect, (float)start, (float)sweep);
            e.Graphics.DrawPie(Pens.White, rect, (float)start, (float)sweep);
            start += sweep;
        }

        e.Graphics.DrawString("Задолженность по дням", new Font(Font, FontStyle.Bold), Brushes.Black, 20, 20);
        DrawDebtLegend(e.Graphics, values, total, colors);
    }

    private void DrawDebtLegend(Graphics graphics, List<decimal> values, decimal total, Color[] colors)
    {
        var legendX = 18;
        var legendY = 270;
        var rowHeight = 22;
        var columnWidth = 178;
        var rowsPerColumn = 9;
        var maxItems = Math.Min(values.Count, rowsPerColumn * 2);

        for (var i = 0; i < maxItems; i++)
        {
            var row = _lastDebt.Rows[i];
            var date = FormatReportDate(row["Дата"]);
            var amount = values[i];
            var percent = amount / total * 100;
            var column = i / rowsPerColumn;
            var rowIndex = i % rowsPerColumn;
            var x = legendX + column * columnWidth;
            var y = legendY + rowIndex * rowHeight;

            using var brush = new SolidBrush(colors[i % colors.Length]);
            graphics.FillRectangle(brush, x, y + 3, 12, 12);
            graphics.DrawRectangle(Pens.Gray, x, y + 3, 12, 12);
            graphics.DrawString($"{date}: {amount:0} ({percent:0.#}%)", Font, Brushes.Black, x + 18, y);
        }

        if (values.Count > maxItems)
        {
            graphics.DrawString($"Еще дней: {values.Count - maxItems}", Font, Brushes.DimGray, legendX, legendY + rowsPerColumn * rowHeight + 4);
        }
    }

    private static string FormatReportDate(object value)
    {
        return value switch
        {
            DateOnly date => date.ToString("dd.MM"),
            DateTime date => date.ToString("dd.MM"),
            _ => value.ToString() ?? ""
        };
    }

    private sealed class GroupItem
    {
        public GroupItem(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        private string Name { get; }
        public override string ToString() => Name;
    }
}
