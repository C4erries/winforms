namespace StationeryStore;

public static class UiStyles
{
    public static void ApplyPage(TabPage page)
    {
        page.BackColor = Color.WhiteSmoke;
        page.Padding = new Padding(10);
    }

    public static void ApplyGrid(DataGridView grid)
    {
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.FixedSingle;
        grid.RowHeadersVisible = false;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(238, 238, 238);
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font(grid.Font, FontStyle.Bold);
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 225, 245);
        grid.DefaultCellStyle.SelectionForeColor = Color.Black;
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 238, 238);
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.ReadOnly = true;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.RowTemplate.Height = 28;
        grid.ColumnHeadersHeight = 32;
    }

    public static FlowLayoutPanel ButtonPanel()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 58,
            Padding = new Padding(8, 10, 8, 8),
            BackColor = Color.WhiteSmoke
        };
    }

    public static Button Button(string text, int width = 124)
    {
        return new Button
        {
            Text = text,
            Width = width,
            Height = 32,
            Margin = new Padding(4, 0, 6, 0),
            TextAlign = ContentAlignment.MiddleCenter,
            UseVisualStyleBackColor = true
        };
    }

    public static Label SectionTitle(string text)
    {
        return new Label
        {
            Text = text,
            Dock = DockStyle.Top,
            Height = 28,
            Padding = new Padding(8, 7, 0, 0),
            Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold),
            BackColor = Color.White
        };
    }

    public static void HideTechnicalColumns(DataGridView grid, params string[] extraNames)
    {
        var names = new HashSet<string>(extraNames, StringComparer.OrdinalIgnoreCase)
        {
            "id"
        };

        foreach (DataGridViewColumn column in grid.Columns)
        {
            var propertyName = column.DataPropertyName;
            if (names.Contains(column.Name)
                || names.Contains(propertyName)
                || column.Name.EndsWith("_id", StringComparison.OrdinalIgnoreCase)
                || propertyName.EndsWith("_id", StringComparison.OrdinalIgnoreCase))
            {
                column.Visible = false;
            }
        }
    }
}
