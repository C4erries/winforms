namespace WinFormsApp3
{
    partial class AddFuturaForm
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
            comboBoxClientName = new ComboBox();
            dateTimePicker1 = new DateTimePicker();
            label1 = new Label();
            label2 = new Label();
            buttonYes = new Button();
            buttonNo = new Button();
            SuspendLayout();
            // 
            // comboBoxClientName
            // 
            comboBoxClientName.FormattingEnabled = true;
            comboBoxClientName.Location = new Point(129, 47);
            comboBoxClientName.Margin = new Padding(4, 5, 4, 5);
            comboBoxClientName.Name = "comboBoxClientName";
            comboBoxClientName.Size = new Size(171, 33);
            comboBoxClientName.TabIndex = 0;
            comboBoxClientName.SelectedIndexChanged += comboBoxClientName_SelectedIndexChanged;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(129, 137);
            dateTimePicker1.Margin = new Padding(4, 5, 4, 5);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(284, 31);
            dateTimePicker1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(41, 50);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(67, 25);
            label1.TabIndex = 2;
            label1.Text = "Клиент";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(41, 147);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(49, 25);
            label2.TabIndex = 3;
            label2.Text = "Дата";
            // 
            // buttonYes
            // 
            buttonYes.Location = new Point(68, 373);
            buttonYes.Margin = new Padding(4, 5, 4, 5);
            buttonYes.Name = "buttonYes";
            buttonYes.Size = new Size(127, 38);
            buttonYes.TabIndex = 4;
            buttonYes.Text = "Подтвердить";
            buttonYes.UseVisualStyleBackColor = true;
            buttonYes.Click += buttonYes_Click;
            // 
            // buttonNo
            // 
            buttonNo.Location = new Point(347, 373);
            buttonNo.Margin = new Padding(4, 5, 4, 5);
            buttonNo.Name = "buttonNo";
            buttonNo.Size = new Size(129, 38);
            buttonNo.TabIndex = 5;
            buttonNo.Text = "Отмена";
            buttonNo.UseVisualStyleBackColor = true;
            buttonNo.Click += buttonNo_Click;
            // 
            // AddFuturaForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1143, 440);
            Controls.Add(buttonNo);
            Controls.Add(buttonYes);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dateTimePicker1);
            Controls.Add(comboBoxClientName);
            Margin = new Padding(4, 5, 4, 5);
            Name = "AddFuturaForm";
            Text = "AddFuturaForm";
            Load += AddFuturaForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboBoxClientName;
        private DateTimePicker dateTimePicker1;
        private Label label1;
        private Label label2;
        private Button buttonYes;
        private Button buttonNo;
    }
}