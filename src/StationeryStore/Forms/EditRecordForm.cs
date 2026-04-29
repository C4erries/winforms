using StationeryStore.Models;

namespace StationeryStore;

public sealed class EditRecordForm : Form
{
    private readonly Dictionary<string, Control> _controls = new();

    public EditRecordForm(string title)
    {
        Text = title;
        StartPosition = FormStartPosition.CenterParent;
        Width = 460;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.WhiteSmoke;
    }

    public void AddText(string key, string caption, string value = "")
    {
        var textBox = new TextBox { Text = value, Width = 260 };
        AddRow(key, caption, textBox);
    }

    public void AddDecimal(string key, string caption, decimal value = 0)
    {
        var numeric = new NumericUpDown
        {
            DecimalPlaces = 2,
            Maximum = 100000000,
            Value = Math.Min(100000000, Math.Max(0, value)),
            Width = 160
        };
        AddRow(key, caption, numeric);
    }

    public void AddDate(string key, string caption, DateTime value)
    {
        var picker = new DateTimePicker { Value = value, Format = DateTimePickerFormat.Short, Width = 160 };
        AddRow(key, caption, picker);
    }

    public void AddLookup(string key, string caption, IEnumerable<LookupItem> items, int selectedId = 0)
    {
        var combo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 260,
            DataSource = items.ToList()
        };
        if (selectedId > 0)
        {
            combo.SelectedItem = combo.Items.Cast<LookupItem>().FirstOrDefault(x => x.Id == selectedId);
        }
        AddRow(key, caption, combo);
    }

    public string TextValue(string key) => ((TextBox)_controls[key]).Text.Trim();
    public decimal DecimalValue(string key) => ((NumericUpDown)_controls[key]).Value;
    public DateTime DateValue(string key) => ((DateTimePicker)_controls[key]).Value.Date;
    public int LookupId(string key) => ((LookupItem)((ComboBox)_controls[key]).SelectedItem!).Id;

    public void BuildButtons()
    {
        var ok = new Button { Text = "Сохранить", DialogResult = DialogResult.OK, Width = 100 };
        var cancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Width = 100 };
        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            FlowDirection = FlowDirection.RightToLeft,
            Height = 48,
            Padding = new Padding(10),
            BackColor = Color.WhiteSmoke
        };
        panel.Controls.Add(cancel);
        panel.Controls.Add(ok);
        Controls.Add(panel);
        AcceptButton = ok;
        CancelButton = cancel;
        Height = 120 + _controls.Count * 38;
    }

    private void AddRow(string key, string caption, Control editor)
    {
        var y = 14 + _controls.Count * 36;
        var label = new Label { Text = caption, Left = 12, Top = y + 4, Width = 150 };
        editor.Left = 170;
        editor.Top = y;
        Controls.Add(label);
        Controls.Add(editor);
        _controls[key] = editor;
    }
}
