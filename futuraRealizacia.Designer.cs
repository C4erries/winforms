namespace WinFormsApp3
{
    partial class futuraRealizacia
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            comboBox1 = new ComboBox();
            buttonYes = new Button();
            label1 = new Label();
            dataGridView1 = new DataGridView();
            buttonNo = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(229, 64);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(182, 33);
            comboBox1.TabIndex = 0;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // buttonYes
            // 
            buttonYes.Location = new Point(88, 371);
            buttonYes.Name = "buttonYes";
            buttonYes.Size = new Size(112, 34);
            buttonYes.TabIndex = 1;
            buttonYes.Text = "Выбрать";
            buttonYes.UseVisualStyleBackColor = true;
            buttonYes.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(47, 69);
            label1.Name = "label1";
            label1.Size = new Size(67, 25);
            label1.TabIndex = 2;
            label1.Text = "Клиент";
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(229, 123);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 62;
            dataGridView1.Size = new Size(519, 225);
            dataGridView1.TabIndex = 3;
            // 
            // buttonNo
            // 
            buttonNo.Location = new Point(583, 380);
            buttonNo.Name = "buttonNo";
            buttonNo.Size = new Size(112, 34);
            buttonNo.TabIndex = 4;
            buttonNo.Text = "Закрыть";
            buttonNo.UseVisualStyleBackColor = true;
            buttonNo.Click += button2_Click;
            // 
            // futuraRealizacia
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonNo);
            Controls.Add(dataGridView1);
            Controls.Add(label1);
            Controls.Add(buttonYes);
            Controls.Add(comboBox1);
            Name = "futuraRealizacia";
            Text = "futuraRealizacia";
            Load += futuraRealizacia_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboBox1;
        private Button buttonYes;
        private Label label1;
        private DataGridView dataGridView1;
        private Button buttonNo;
    }
}