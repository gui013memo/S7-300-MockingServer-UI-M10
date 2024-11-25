namespace S7_300_MockingServer_UI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            setReadRequestButton = new Button();
            engineNumberTextBox = new TextBox();
            label1 = new Label();
            label3 = new Label();
            label4 = new Label();
            SuspendLayout();
            // 
            // setReadRequestButton
            // 
            setReadRequestButton.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            setReadRequestButton.Location = new Point(30, 89);
            setReadRequestButton.Name = "setReadRequestButton";
            setReadRequestButton.Size = new Size(264, 69);
            setReadRequestButton.TabIndex = 0;
            setReadRequestButton.Text = "Set ReadRequest = 1";
            setReadRequestButton.UseVisualStyleBackColor = true;
            setReadRequestButton.Click += setReadRequestButton_Click;
            // 
            // engineNumberTextBox
            // 
            engineNumberTextBox.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            engineNumberTextBox.Location = new Point(31, 44);
            engineNumberTextBox.MaxLength = 16;
            engineNumberTextBox.Multiline = true;
            engineNumberTextBox.Name = "engineNumberTextBox";
            engineNumberTextBox.Size = new Size(264, 39);
            engineNumberTextBox.TabIndex = 1;
            engineNumberTextBox.Text = "BB42F3LAQL123456";
            engineNumberTextBox.TextAlign = HorizontalAlignment.Center;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(31, 9);
            label1.Name = "label1";
            label1.Size = new Size(182, 32);
            label1.TabIndex = 2;
            label1.Text = "Engine Number";
            label1.DoubleClick += label1_DoubleClick;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(219, 18);
            label3.Name = "label3";
            label3.Size = new Size(71, 21);
            label3.TabIndex = 7;
            label3.Text = "(Max 16)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point);
            label4.ForeColor = Color.Firebrick;
            label4.Location = new Point(32, 161);
            label4.Name = "label4";
            label4.Size = new Size(263, 96);
            label4.TabIndex = 8;
            label4.Text = "M10 - OP50M2 Version\r\nPLC IP: 172.16.100.51\r\nDB:181 / IF Ref: M10";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(327, 263);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(engineNumberTextBox);
            Controls.Add(setReadRequestButton);
            Name = "Form1";
            Text = "S7-300 TMS Mocking Server";
            FormClosing += Form1_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button setReadRequestButton;
        private TextBox engineNumberTextBox;
        private Label label1;
        private Label label3;
        private Label label4;
    }
}
