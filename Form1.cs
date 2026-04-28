 using Npgsql;
namespace WinFormsApp3

{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public NpgsqlConnection con;
        public void MyLoad()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION_STRING")
                ?? "Host=localhost;Port=5432;Username=postgres;Password=PAROL12345;Database=forma";

            con = new NpgsqlConnection(connectionString);
            con.Open();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MyLoad();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormProduct fp = new FormProduct(con);
            fp.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormClient fp1 = new FormClient(con);
            fp1.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormFutura fp2 = new FormFutura(con);
            fp2.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            futuraRealizacia fp3 = new futuraRealizacia(con);
            fp3.ShowDialog();
        }
    }
}
