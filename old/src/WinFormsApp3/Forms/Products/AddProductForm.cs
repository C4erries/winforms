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
    public partial class AddProductForm : Form
    {
        public NpgsqlConnection con;
        int id;
        public AddProductForm(NpgsqlConnection con, int id)
        {
            this.con = con;
            this.id = id;
            InitializeComponent();
        }
        public AddProductForm(NpgsqlConnection con, int id, string nameP, string ed)
        {
            InitializeComponent();
            textBoxName.Text = nameP;
            textBoxEd.Text = ed;
            this.con = con;
            this.id = id;
        }

        private void buttonNO_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            if (id == -1)
            {
                try
                {
                    NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Product(name,ed) VALUES (:name,:ed)", con);
                    command.Parameters.AddWithValue("name", textBoxName.Text);
                    command.Parameters.AddWithValue("ed", textBoxEd.Text);
                    command.ExecuteNonQuery();
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    NpgsqlCommand command = new NpgsqlCommand("UPDATE product SET name=:name, ed=:ed WHERE ID=:id", con);
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("name", textBoxName.Text);
                    command.Parameters.AddWithValue("ed", textBoxEd.Text);
                    command.ExecuteNonQuery();
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
