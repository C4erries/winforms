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
    public partial class FormFutura : Form
    {
        public NpgsqlConnection con;
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();

        int futuraId;
        public FormFutura(NpgsqlConnection con)
        {
            this.con = con;
            InitializeComponent();

        }
        private void Update()
        {
            String sql = "Select futura.ID,futura.data,client.name FROM futura JOIN client ON futura.IDclient=client.ID";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
        }

        private void Update2(int futuraId)
        {
            string sql = @"SELECT fi.id, p.name AS Товар, p.ed AS Ед_изм, fi.quantity AS Количество,
           fi.price AS Цена FROM FuturaInfo fi JOIN Product p ON fi.idProduct = p.id WHERE fi.idFutura = " + futuraId;
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
            DataTable dt2 = new DataTable();

            da.Fill(dt2);
            dataGridView2.DataSource = dt2;
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormFutura_Load(object sender, EventArgs e)
        {
            Update();
            Update2(0);
        }

        private void наклоднаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFuturaForm f = new AddFuturaForm(con);
            f.ShowDialog();
            Update();
        }

        private void товарToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int Id = (int)dataGridView1.CurrentRow.Cells["ID"].Value;
            AddFuturaInfo f = new AddFuturaInfo(con, Id);
            f.ShowDialog();
            Update2(Id);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int Id = (int)dataGridView1.CurrentRow.Cells["ID"].Value;
            Update2(Id);
        }

        private void наклоднаяToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddFuturaForm f = new AddFuturaForm(con);
            f.ShowDialog();
            Update();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int Id = (int)dataGridView1.CurrentRow.Cells["ID"].Value;
            Update2(Id);
        }
    }
}
