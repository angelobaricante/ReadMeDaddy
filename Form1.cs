using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json.Linq; // Ensure Newtonsoft.Json is installed via NuGet

namespace ReadMeDaddy
{
    public partial class Form1 : Form
    {
        private ApiHandler apiHandler;
        private string filePath; // To store the path of the file selected by the user

        public Form1()
        {
            InitializeComponent();
            var apiKey = LoadApiKey();
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("API key is not configured properly.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); // Optionally close the application if API key is not found
            }
            apiHandler = new ApiHandler(apiKey); // Ensure API key security
        }

        private string LoadApiKey()
        {
            try
            {
                // Adjust the relative path to point from the bin\Debug or bin\Release directory to the project root
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\config.json");
                var json = File.ReadAllText(path);
                return JObject.Parse(json)["ApiKey"].ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to read API Key: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }


        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                fileLabel.Text = filePath; // Display selected file path
                processButton.Enabled = true; // Enable the Process button
            }
        }

        private async void ProcessButton_Click(object sender, EventArgs e)
        {
            string prompt = taskInputTextBox.Text;
            if (string.IsNullOrEmpty(prompt))
            {
                MessageBox.Show("Please enter a task.", "Task Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("No file selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string fileContent = FileOperations.ReadTextFromFile(filePath);
            if (fileContent.StartsWith("Error reading file:"))
            {
                outputTextBox.Text = fileContent; // Display file reading errors directly
                return;
            }

            outputTextBox.Text = "Processing...";
            try
            {
                string aiGeneratedContent = await apiHandler.SendRequestToOpenAI(prompt, fileContent);
                outputTextBox.Text = aiGeneratedContent;
                updateButton.Enabled = true; // Enable the Update button after successful processing
            }
            catch (Exception ex)
            {
                outputTextBox.Text = "Error processing request: " + ex.Message;
                updateButton.Enabled = false;
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                FileOperations.AppendTextToFile(filePath, outputTextBox.Text);
                MessageBox.Show("Content successfully updated to file.", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
