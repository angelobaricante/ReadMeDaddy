namespace ReadMeDaddy
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Read Me Daddy: AI File Assistant";

            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.fileLabel = new System.Windows.Forms.Label();
            this.processButton = new System.Windows.Forms.Button();
            this.taskInputTextBox = new System.Windows.Forms.TextBox();
            this.updateButton = new System.Windows.Forms.Button();
            this.selectFileButton = new System.Windows.Forms.Button();

            // 
            // outputTextBox
            // 
            this.outputTextBox.Location = new System.Drawing.Point(10, 110);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.Size = new System.Drawing.Size(760, 300);
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            // 
            // fileLabel
            // 
            this.fileLabel.AutoSize = true;
            this.fileLabel.Location = new System.Drawing.Point(10, 60);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(150, 20);
            this.fileLabel.Text = "No file selected";

            // 
            // processButton
            // 
            this.processButton.Location = new System.Drawing.Point(520, 20);
            this.processButton.Name = "processButton";
            this.processButton.Size = new System.Drawing.Size(100, 30);
            this.processButton.Text = "Process";
            this.processButton.UseVisualStyleBackColor = true;
            this.processButton.Click += new System.EventHandler(this.ProcessButton_Click);

            // 
            // taskInputTextBox
            // 
            this.taskInputTextBox.Location = new System.Drawing.Point(120, 20);
            this.taskInputTextBox.Name = "taskInputTextBox";
            this.taskInputTextBox.Size = new System.Drawing.Size(380, 26);

            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(630, 20);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(100, 30);
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.UpdateButton_Click);

            // 
            // selectFileButton
            // 
            this.selectFileButton.Location = new System.Drawing.Point(10, 20);
            this.selectFileButton.Name = "selectFileButton";
            this.selectFileButton.Size = new System.Drawing.Size(100, 30);
            this.selectFileButton.Text = "Select File";
            this.selectFileButton.UseVisualStyleBackColor = true;
            this.selectFileButton.Click += new System.EventHandler(this.SelectFileButton_Click);

            // Add controls to the form
            this.Controls.Add(this.selectFileButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.processButton);
            this.Controls.Add(this.taskInputTextBox);
            this.Controls.Add(this.fileLabel);
            this.Controls.Add(this.outputTextBox);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Label fileLabel;
        private System.Windows.Forms.Button processButton;
        private System.Windows.Forms.TextBox taskInputTextBox;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button selectFileButton;
    }
}
