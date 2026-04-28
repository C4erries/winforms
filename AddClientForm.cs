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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinFormsApp3
{
    public partial class AddClientForm : Form
    {
        public NpgsqlConnection con;
        int id;
        public AddClientForm(NpgsqlConnection con, int id)
        {
            this.con = con;
            this.id = id;
            InitializeComponent();
        }
        public AddClientForm(NpgsqlConnection con, int id, string nameP, string adress, string phone)
        {
            InitializeComponent();
            textBoxName.Text = nameP;
            textBoxAdres.Text = adress;
            textBoxPhone.Text = phone;
            this.con = con;
            this.id = id;
        }
        private void buttonNo_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            if (id == -1)
            {
                try
                {
                    NpgsqlCommand command = new NpgsqlCommand("INSERT INTO client(name,address,phone) VALUES (:name,:address,:phone)", con);
                    command.Parameters.AddWithValue("name", textBoxName.Text);
                    command.Parameters.AddWithValue("address", textBoxAdres.Text);
                    command.Parameters.AddWithValue("phone", textBoxPhone.Text);
                    command.ExecuteNonQuery();
                    Close();
                }
                catch { }
            }
            else
            {
                try
                {
                    NpgsqlCommand command = new NpgsqlCommand("UPDATE client SET name=:name, address=:address, phone=:phone WHERE ID=:id", con);
                    command.Parameters.AddWithValue("id", id);
                    command.Parameters.AddWithValue("name", textBoxName.Text);
                    command.Parameters.AddWithValue("address", textBoxAdres.Text);
                    command.Parameters.AddWithValue("phone", textBoxPhone.Text);
                    command.ExecuteNonQuery();
                    Close();
                }
                catch { }
            }
        }
    }
}
