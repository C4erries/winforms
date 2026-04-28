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
    public partial class AddFuturaInfo : Form
    {
        public NpgsqlConnection con;
        DataTable dt = new DataTable();
        DataSet ds = new DataSet();
        private int futuraId;
        public AddFuturaInfo(NpgsqlConnection con, int futuraId)
        {
            InitializeComponent();
            this.con = con;
            this.futuraId = futuraId;
        }

        private void Update()
        {
            String sql = "Select*from product";
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, con);
            ds.Reset();
            da.Fill(ds);
            dt = ds.Tables[0];
            comboBoxProduct.DataSource = dt;
            comboBoxProduct.DisplayMember = "name";
            comboBoxProduct.ValueMember = "ID";
        }

        private void Yes_Click(object sender, EventArgs e)
        {
            try
            {
                NpgsqlCommand command = new NpgsqlCommand("INSERT INTO FuturaInfo (idFutura, idProduct, quantity, price)" +
                    " VALUES (:idFutura, :idProduct, :quantity, :price) RETURNING id", con);
                command.Parameters.AddWithValue("idFutura", futuraId);
                command.Parameters.AddWithValue("idProduct", comboBoxProduct.SelectedValue);
                command.Parameters.AddWithValue("quantity", Convert.ToDouble(textBoxQuantity.Text));
                command.Parameters.AddWithValue("price", Convert.ToDouble(textBoxPrice.Text));
                command.ExecuteNonQuery();

                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении: " + ex.Message);
            }
        }
        private void AddFuturaInfo_Load(object sender, EventArgs e)
        {
            Update();
        }
        private void buttonNo_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBoxProduct_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
