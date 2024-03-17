namespace ImageSteganography
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
            encodingPath = new TextBox();
            button1 = new Button();
            button2 = new Button();
            encodeMessage = new TextBox();
            textBox3 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // encodingPath
            // 
            encodingPath.AcceptsTab = true;
            encodingPath.Location = new Point(178, 125);
            encodingPath.Name = "encodingPath";
            encodingPath.PlaceholderText = "path...";
            encodingPath.Size = new Size(196, 23);
            encodingPath.TabIndex = 0;
            encodingPath.TabStop = false;
            encodingPath.TextChanged += textBox1_TextChanged;
            // 
            // button1
            // 
            button1.Location = new Point(299, 198);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 1;
            button1.Text = "Encode";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(407, 198);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 2;
            button2.Text = "Decode";
            button2.UseVisualStyleBackColor = true;
            // 
            // encodeMessage
            // 
            encodeMessage.Location = new Point(178, 154);
            encodeMessage.Multiline = true;
            encodeMessage.Name = "encodeMessage";
            encodeMessage.PlaceholderText = "message...";
            encodeMessage.Size = new Size(196, 38);
            encodeMessage.TabIndex = 3;
            encodeMessage.TextChanged += textBox2_TextChanged;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(407, 125);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "path...";
            textBox3.Size = new Size(196, 23);
            textBox3.TabIndex = 4;
            textBox3.TextChanged += textBox3_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(242, 90);
            label1.Name = "label1";
            label1.Size = new Size(74, 21);
            label1.TabIndex = 5;
            label1.Text = "Encoding";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(467, 90);
            label2.Name = "label2";
            label2.Size = new Size(76, 21);
            label2.TabIndex = 6;
            label2.Text = "Decoding";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox3);
            Controls.Add(encodeMessage);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(encodingPath);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox encodingPath;
        private Button button1;
        private Button button2;
        private TextBox encodeMessage;
        private TextBox textBox3;
        private Label label1;
        private Label label2;
    }
}
