namespace WinFormsApp3
{
    partial class AddFuturaInfo
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
            comboBoxProduct = new ComboBox();
            buttonYes = new Button();
            buttonNo = new Button();
            label1 = new Label();
            textBoxQuantity = new TextBox();
            textBoxPrice = new TextBox();
            label3 = new Label();
            label4 = new Label();
            SuspendLayout();
            // 
            // comboBoxProduct
            // 
            comboBoxProduct.FormattingEnabled = true;
            comboBoxProduct.Location = new Point(224, 47);
            comboBoxProduct.Name = "comboBoxProduct";
            comboBoxProduct.Size = new Size(182, 33);
            comboBoxProduct.TabIndex = 0;
            comboBoxProduct.SelectedIndexChanged += comboBoxProduct_SelectedIndexChanged;
            // 
            // buttonYes
            // 
            buttonYes.Location = new Point(77, 379);
            buttonYes.Name = "buttonYes";
            buttonYes.Size = new Size(143, 34);
            buttonYes.TabIndex = 1;
            buttonYes.Text = "Подтвердить";
            buttonYes.UseVisualStyleBackColor = true;
            buttonYes.Click += Yes_Click;
            // 
            // buttonNo
            // 
            buttonNo.Location = new Point(376, 379);
            buttonNo.Name = "buttonNo";
            buttonNo.Size = new Size(112, 34);
            buttonNo.TabIndex = 2;
            buttonNo.Text = "Отмена";
            buttonNo.UseVisualStyleBackColor = true;
            buttonNo.Click += buttonNo_Click_1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(26, 47);
            label1.Name = "label1";
            label1.Size = new Size(62, 25);
            label1.TabIndex = 4;
            label1.Text = "Товар";
            // 
            // textBoxQuantity
            // 
            textBoxQuantity.Location = new Point(224, 120);
            textBoxQuantity.Name = "textBoxQuantity";
            textBoxQuantity.Size = new Size(150, 31);
            textBoxQuantity.TabIndex = 6;
            // 
            // textBoxPrice
            // 
            textBoxPrice.Location = new Point(224, 206);
            textBoxPrice.Name = "textBoxPrice";
            textBoxPrice.Size = new Size(150, 31);
            textBoxPrice.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(26, 120);
            label3.Name = "label3";
            label3.Size = new Size(107, 25);
            label3.TabIndex = 8;
            label3.Text = "Количество";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(22, 212);
            label4.Name = "label4";
            label4.Size = new Size(53, 25);
            label4.TabIndex = 9;
            label4.Text = "Цена";
            // 
            // AddFuturaInfo
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1279, 463);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(textBoxPrice);
            Controls.Add(textBoxQuantity);
            Controls.Add(label1);
            Controls.Add(buttonNo);
            Controls.Add(buttonYes);
            Controls.Add(comboBoxProduct);
            Name = "AddFuturaInfo";
            Text = "AddFuturaInfo";
            Load += AddFuturaInfo_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboBoxProduct;
        private Button buttonYes;
        private Button buttonNo;
        private Label label1;
        private TextBox textBoxQuantity;
        private TextBox textBoxPrice;
        private Label label3;
        private Label label4;
    }
}