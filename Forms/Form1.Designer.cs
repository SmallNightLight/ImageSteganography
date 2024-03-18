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
            EncodingPath = new TextBox();
            EncodeButton = new Button();
            DecodeButton = new Button();
            EncodingMessage = new TextBox();
            DecodingPath = new TextBox();
            EncodingLabel = new Label();
            DecodingLabel = new Label();
            DecodingMessage = new TextBox();
            OpenEncodeFile = new Button();
            OpenDecodeFile = new Button();
            EncodeImageName = new Label();
            DecodeImageName = new Label();
            SuspendLayout();
            // 
            // EncodingPath
            // 
            EncodingPath.AcceptsTab = true;
            EncodingPath.Location = new Point(184, 126);
            EncodingPath.Name = "EncodingPath";
            EncodingPath.PlaceholderText = "path...";
            EncodingPath.Size = new Size(166, 23);
            EncodingPath.TabIndex = 0;
            EncodingPath.TabStop = false;
            // 
            // EncodeButton
            // 
            EncodeButton.Location = new Point(305, 311);
            EncodeButton.Name = "EncodeButton";
            EncodeButton.Size = new Size(75, 23);
            EncodeButton.TabIndex = 1;
            EncodeButton.Text = "Encode";
            EncodeButton.UseVisualStyleBackColor = true;
            // 
            // DecodeButton
            // 
            DecodeButton.Location = new Point(413, 311);
            DecodeButton.Name = "DecodeButton";
            DecodeButton.Size = new Size(75, 23);
            DecodeButton.TabIndex = 2;
            DecodeButton.Text = "Decode";
            DecodeButton.UseVisualStyleBackColor = true;
            // 
            // EncodingMessage
            // 
            EncodingMessage.Location = new Point(184, 193);
            EncodingMessage.Multiline = true;
            EncodingMessage.Name = "EncodingMessage";
            EncodingMessage.PlaceholderText = "message...";
            EncodingMessage.Size = new Size(196, 112);
            EncodingMessage.TabIndex = 3;
            // 
            // DecodingPath
            // 
            DecodingPath.Location = new Point(413, 126);
            DecodingPath.Name = "DecodingPath";
            DecodingPath.PlaceholderText = "path...";
            DecodingPath.Size = new Size(165, 23);
            DecodingPath.TabIndex = 4;
            // 
            // EncodingLabel
            // 
            EncodingLabel.AutoSize = true;
            EncodingLabel.Font = new Font("Segoe UI", 12F);
            EncodingLabel.Location = new Point(242, 90);
            EncodingLabel.Name = "EncodingLabel";
            EncodingLabel.Size = new Size(74, 21);
            EncodingLabel.TabIndex = 5;
            EncodingLabel.Text = "Encoding";
            // 
            // DecodingLabel
            // 
            DecodingLabel.AutoSize = true;
            DecodingLabel.Font = new Font("Segoe UI", 12F);
            DecodingLabel.Location = new Point(473, 91);
            DecodingLabel.Name = "DecodingLabel";
            DecodingLabel.Size = new Size(76, 21);
            DecodingLabel.TabIndex = 6;
            DecodingLabel.Text = "Decoding";
            // 
            // DecodingMessage
            // 
            DecodingMessage.Enabled = false;
            DecodingMessage.Location = new Point(412, 193);
            DecodingMessage.Multiline = true;
            DecodingMessage.Name = "DecodingMessage";
            DecodingMessage.PlaceholderText = "message...";
            DecodingMessage.ReadOnly = true;
            DecodingMessage.Size = new Size(196, 112);
            DecodingMessage.TabIndex = 7;
            // 
            // OpenEncodeFile
            // 
            OpenEncodeFile.Location = new Point(353, 125);
            OpenEncodeFile.Name = "OpenEncodeFile";
            OpenEncodeFile.Size = new Size(27, 25);
            OpenEncodeFile.TabIndex = 8;
            OpenEncodeFile.Text = ">";
            OpenEncodeFile.UseVisualStyleBackColor = true;
            OpenEncodeFile.Click += OpenEncodeFile_Click;
            // 
            // OpenDecodeFile
            // 
            OpenDecodeFile.Location = new Point(581, 125);
            OpenDecodeFile.Name = "OpenDecodeFile";
            OpenDecodeFile.Size = new Size(27, 25);
            OpenDecodeFile.TabIndex = 9;
            OpenDecodeFile.Text = ">";
            OpenDecodeFile.UseVisualStyleBackColor = true;
            OpenDecodeFile.Click += OpenDecodeFile_Click;
            // 
            // EncodeImageName
            // 
            EncodeImageName.AutoSize = true;
            EncodeImageName.Location = new Point(184, 152);
            EncodeImageName.Name = "EncodeImageName";
            EncodeImageName.Size = new Size(0, 15);
            EncodeImageName.TabIndex = 10;
            // 
            // DecodeImageName
            // 
            DecodeImageName.AutoSize = true;
            DecodeImageName.Location = new Point(413, 152);
            DecodeImageName.Name = "DecodeImageName";
            DecodeImageName.Size = new Size(0, 15);
            DecodeImageName.TabIndex = 11;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(DecodeImageName);
            Controls.Add(EncodeImageName);
            Controls.Add(OpenDecodeFile);
            Controls.Add(OpenEncodeFile);
            Controls.Add(DecodingMessage);
            Controls.Add(DecodingLabel);
            Controls.Add(EncodingLabel);
            Controls.Add(DecodingPath);
            Controls.Add(EncodingMessage);
            Controls.Add(DecodeButton);
            Controls.Add(EncodeButton);
            Controls.Add(EncodingPath);
            Name = "Form1";
            Text = "        ";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private TextBox EncodingPath;
        private Button EncodeButton;
        private Button DecodeButton;
        private TextBox EncodingMessage;
        private TextBox DecodingPath;
        private Label EncodingLabel;
        private Label DecodingLabel;
        private TextBox DecodingMessage;
        private Button OpenEncodeFile;
        private Button OpenDecodeFile;
        private Label EncodeImageName;
        private Label DecodeImageName;
    }
}
