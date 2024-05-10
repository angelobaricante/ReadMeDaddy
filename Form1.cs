using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using AIFileAssistant;
using Newtonsoft.Json.Linq; // Ensure Newtonsoft.Json is installed via NuGet

namespace ReadMeDaddy
{
    public partial class Form1 : Form
    {
        private Form2 settingsForm;
        private ApiHandler apiHandler;
        private string filePath; // To store the path of the file selected by the user
        private string fileContent; // To store the content of the file

        public Form1()
        {
            InitializeComponent();
            InitializeRichTextBox();
            settingsForm = new Form2();
            settingsButton.Click += SettingsButton_Click;
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
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                fileLabel.Text = filePath; // Display selected file path
                fileContent = FileOperations.ReadTextFromFile(filePath);
                if (fileContent.StartsWith("Error reading file:") || fileContent.StartsWith("Unsupported file format."))
                {
                    MessageBox.Show(fileContent, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    processButton.Enabled = false;
                }
                else
                {
                    processButton.Enabled = true; // Enable the Process button
                }
            }
        }

        private async void ProcessButton_Click(object sender, EventArgs e)
        {
            string prompt = taskInputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(prompt))
            {
                MessageBox.Show("Please enter a prompt.", "Prompt Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(fileContent))
            {
                MessageBox.Show("File content is empty or not loaded properly.", "Content Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Display user message in the chat interface
            AppendTextToChat("You: \n" + prompt, true);

            taskInputTextBox.Clear();  // Clear the input box after sending the message

            try
            {
                string aiGeneratedContent = await apiHandler.SendRequestToOpenAI(fileContent, prompt);
                if (!string.IsNullOrEmpty(aiGeneratedContent))
                {
                    AppendTextToChat("ReadMeDaddy: \n" + aiGeneratedContent, false);
                    copyButton.Enabled = true; // Enable the Update button
                }
                else
                {
                    MessageBox.Show("Received empty response from API.", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing request: " + ex.Message, "Processing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppendTextToChat("ReadMeDaddy: \nError: " + ex.Message, false);
                copyButton.Enabled = false;
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Append the last interaction to the file
                FileOperations.AppendTextToFile(filePath, richTextBox1.Text);
                MessageBox.Show("Content successfully updated to file.", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool lastMessageWasUser = true;  // A flag to track who sent the last message

        private void AppendTextToChat(string text, bool isUser)
        {
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;

            // Add an extra newline if the last message was not from the same sender to space out entries
            if (lastMessageWasUser != isUser)
            {
                richTextBox1.AppendText("\n");
            }

            // Setting the color explicitly based on whether it's the user or the API
            if (isUser)
            {
                richTextBox1.SelectionColor = Color.FromArgb(26, 188, 156); // Teal color for the user
            }
            else
            {
                richTextBox1.SelectionColor = Color.White; // White color for the API
            }

            richTextBox1.SelectionIndent = isUser ? 0 : 50;  // Indent API messages for right alignment

            ApplyCustomFormatting(text, isUser);

            richTextBox1.ScrollToCaret();  // Ensure the latest message is visible

            lastMessageWasUser = isUser;  // Update the flag to reflect the sender of the current message
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Add any initialization code here if needed
        }

        private void taskInputTextBox_TextChanged(object sender, EventArgs e)
        {
            // Add any code that should run when text changes in taskInputTextBox
        }

        private void InitializeRichTextBox()
        {
            richTextBox1.ReadOnly = true;
            richTextBox1.Multiline = true;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox1.BackColor = Color.FromArgb(33, 33, 33);
        }

        private void ApplyCustomFormatting(string text, bool isUser)
        {
            string[] lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;

                if (line.StartsWith("ReadMeDaddy:") || line.StartsWith("You:"))
                {
                    int colonIndex = line.IndexOf(':');
                    if (colonIndex != -1)
                    {
                        richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                        richTextBox1.AppendText(line.Substring(0, colonIndex + 1) + " ");
                        richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
                        richTextBox1.AppendText(line.Substring(colonIndex + 1) + "\n");
                    }
                }
                else
                {
                    richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
                    richTextBox1.AppendText(line + "\n");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            settingsForm.Show();
        }
    }
}