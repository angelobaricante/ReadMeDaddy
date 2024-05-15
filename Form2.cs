using System;
using System.Windows.Forms;

namespace AIFileAssistant
{
    public partial class Form2 : Form
    {
        public string ApiKey { get; private set; }

        public Form2()
        {
            InitializeComponent();
            exitButton.Click += ExitButton_Click;
            saveButton.Click += saveButton_Click;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
        }

        private void fileLabel_Click(object sender, EventArgs e)
        {
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void apiKeyStorage_TextChanged(object sender, EventArgs e)
        {
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            ApiKey = apiKeyTextBox.Text.Trim();
            if (string.IsNullOrEmpty(ApiKey))
            {
                MessageBox.Show("API key cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void apiKeyTextBox_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
