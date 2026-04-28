using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp3
{
    public partial class futuraRealizacia : Form
    {
        public NpgsqlConnection con;
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        NpgsqlDataReader dr;
        NpgsqlDataAdapter da;
        public futuraRealizacia(NpgsqlConnection con)
        {
            InitializeComponent();
            this.con = con;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void Update()
        {
            String sql = "Select*from client";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "ID";
        }

        private void futuraRealizacia_Load(object sender, EventArgs e)
        {
            Update();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string sql = "SELECT Futura.ID, product.name, quantity * price AS cost FROM futura "+
                "LEFT JOIN futurainfo ON futura.idclient =:idclient AND futurainfo.idfutura = futura.id JOIN Product on product.id = Futurainfo.idproduct";

            NpgsqlCommand command = new NpgsqlCommand(sql, con);
            command.Parameters.AddWithValue("idclient", comboBox1.SelectedValue);
            da = new NpgsqlDataAdapter(command);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            dataGridView1.DataSource = dt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
