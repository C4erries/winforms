using System.Data;
using Npgsql;
using StationeryStore.Data;
using StationeryStore.Models;

namespace StationeryStore;

public sealed class CrudTabPage : TabPage
{
    private readonly Db _db;
    private readonly string _selectSql;
    private readonly Action<EditRecordForm, DataRow?> _buildForm;
    private readonly Func<EditRecordForm, int?, (string Sql, NpgsqlParameter[] Parameters)> _saveCommand;
    private readonly string _deleteSql;
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill };

    public CrudTabPage(
        string title,
        Db db,
        string selectSql,
        Action<EditRecordForm, DataRow?> buildForm,
        Func<EditRecordForm, int?, (string Sql, NpgsqlParameter[] Parameters)> saveCommand,
        string deleteSql)
    {
        Text = title;
        UiStyles.ApplyPage(this);
        UiStyles.ApplyGrid(_grid);
        _db = db;
        _selectSql = selectSql;
        _buildForm = buildForm;
        _saveCommand = saveCommand;
        _deleteSql = deleteSql;

        var buttons = UiStyles.ButtonPanel();
        var add = UiStyles.Button("Добавить");
        var edit = UiStyles.Button("Изменить");
        var delete = UiStyles.Button("Удалить");
        var refresh = UiStyles.Button("Обновить");
        buttons.Controls.AddRange(new Control[] { add, edit, delete, refresh });

        add.Click += (_, _) => OpenEditor(null);
        edit.Click += (_, _) => OpenEditor(CurrentRow());
        delete.Click += (_, _) => DeleteSelected();
        refresh.Click += (_, _) => LoadData();

        var panel = new Panel { Dock = DockStyle.Fill };
        panel.Controls.Add(_grid);
        panel.Controls.Add(buttons);
        panel.Controls.Add(UiStyles.SectionTitle(title));
        Controls.Add(panel);
        LoadData();
    }

    public void LoadData()
    {
        _grid.DataSource = _db.Query(_selectSql);
        UiStyles.HideTechnicalColumns(_grid);
        _grid.ClearSelection();
    }

    public static List<LookupItem> Lookup(Db db, string sql)
    {
        return db.Query(sql).Rows.Cast<DataRow>()
            .Select(row => new LookupItem(Convert.ToInt32(row["id"]), row["name"].ToString() ?? ""))
            .ToList();
    }

    private DataRow? CurrentRow()
    {
        if (_grid.CurrentRow?.DataBoundItem is DataRowView view)
        {
            return view.Row;
        }
        MessageBox.Show("Выберите строку.");
        return null;
    }

    private void OpenEditor(DataRow? row)
    {
        var form = new EditRecordForm(row == null ? "Добавление" : "Изменение");
        _buildForm(form, row);
        form.BuildButtons();
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var id = row == null ? (int?)null : Convert.ToInt32(row["id"]);
        var command = _saveCommand(form, id);
        _db.Execute(command.Sql, command.Parameters);
        LoadData();
    }

    private void DeleteSelected()
    {
        var row = CurrentRow();
        if (row == null)
        {
            return;
        }

        if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        _db.Execute(_deleteSql, new NpgsqlParameter("id", Convert.ToInt32(row["id"])));
        LoadData();
    }
}
