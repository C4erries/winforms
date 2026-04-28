using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp3
{
    public partial class FormClient : Form
    {
        public NpgsqlConnection con;
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        public FormClient(NpgsqlConnection con)
        {
            this.con = con;
            InitializeComponent();
        }
        public void Update()
        {
            String sql = "Select*from Client";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].HeaderText = "Номер";
            dataGridView1.Columns[1].HeaderText = "ФИО";
            dataGridView1.Columns[2].HeaderText = "Адрес";
            dataGridView1.Columns[3].HeaderText = "Номер Телефона";
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        private void FormClient_Load(object sender, EventArgs e)
        {
            Update();
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddClientForm f = new AddClientForm(con, -1);
            f.ShowDialog();
            Update();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                int id = (int)dataGridView1.CurrentRow.Cells["id"].Value;

                DialogResult result = MessageBox.Show("Удалить выбранный продукт?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        NpgsqlCommand command = new NpgsqlCommand("delete from client where id = :id", con);
                        command.Parameters.AddWithValue(":id", id);
                        command.ExecuteNonQuery();
                        RenumberIds();

                        Update();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления!");
            }
        }

        private void RenumberIds()
        {
            try
            {
                NpgsqlCommand selectCmd = new NpgsqlCommand("SELECT id FROM client ORDER BY id", con);
                NpgsqlDataReader reader = selectCmd.ExecuteReader();

                List<int> oldIds = new List<int>();
                while (reader.Read())
                {
                    oldIds.Add(reader.GetInt32(0));
                }
                reader.Close();

                for (int i = 0; i < oldIds.Count; i++)
                {
                    int newId = i + 1;
                    if (oldIds[i] != newId)
                    {
                        NpgsqlCommand updateCmd = new NpgsqlCommand("UPDATE client SET id = :newId WHERE id = :oldId", con);
                        updateCmd.Parameters.AddWithValue(":newId", newId);
                        updateCmd.Parameters.AddWithValue(":oldId", oldIds[i]);
                        updateCmd.ExecuteNonQuery();
                    }
                }

                NpgsqlCommand resetSeqCmd = new NpgsqlCommand("SELECT setval('client_id_seq', (SELECT COALESCE(MAX(id), 1) FROM client))", con);
                resetSeqCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при перенумерации: {ex.Message}");
            }
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = (int)dataGridView1.CurrentRow.Cells["ID"].Value;
            string name = (string)dataGridView1.CurrentRow.Cells["name"].Value;
            string adress = (string)dataGridView1.CurrentRow.Cells["address"].Value;
            string phone = (string)dataGridView1.CurrentRow.Cells["phone"].Value;
            AddClientForm f = new AddClientForm(con, id, name, adress, phone);
            f.ShowDialog();
            Update();
        }

        private void FormClient_Load_1(object sender, EventArgs e)
        {
            Update();
        }
    }
}
