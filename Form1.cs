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
        private ApiHandler apiHandler;
        private string filePath; // To store the path of the file selected by the user
        private string fileContent; // To store the content of the file
        private Form2 form2;

        public Form1()
        {
            InitializeComponent();
            InitializeRichTextBox();

            // Existing event handler subscriptions
            settingsButton.Click += SettingsButton_Click;

            // New event handler subscriptions for clearButton and copyButton
            clearButton.Click += ClearButton_Click;
            copyButton.Click += CopyButton_Click;

            // Show API key input form
            using (var apiKeyForm = new Form2())
            {
                if (apiKeyForm.ShowDialog() == DialogResult.OK)
                {
                    var apiKey = apiKeyForm.ApiKey;
                    apiHandler = new ApiHandler(apiKey); // Initialize with user-provided API key
                }
                else
                {
                    MessageBox.Show("API key is required to use this application.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close(); // Optionally close the application if API key is not provided
                }
            }
        }


        // Event handler for clearButton
        private void ClearButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        // Event handler for copyButton
        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (richTextBox1.TextLength > 0)
            {
                // Find the last occurrence of "ReadMeDaddy:"
                string fullText = richTextBox1.Text;
                int lastIndex = fullText.LastIndexOf("ReadMeDaddy:");

                if (lastIndex != -1)
                {
                    // Extract the message after "ReadMeDaddy:"
                    string message = fullText.Substring(lastIndex + "ReadMeDaddy:".Length).Trim();
                    if (!string.IsNullOrEmpty(message))
                    {
                        Clipboard.SetText(message);
                        MessageBox.Show("Message copied to clipboard successfully.", "Copy Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("The last message from ReadMeDaddy is empty.", "Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("No message from ReadMeDaddy found.", "Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("The text box is empty.", "Copy Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        private void UpdateButton_Click(object sender, EventArgs e)
        {
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
                fileContent = FileOperations.ReadFile(filePath);
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

            // Introduce a delay before showing the typing message
            await Task.Delay(300);

            // Display typing message
            AppendTextToChat("ReadMeDaddy is typing...", false);

            try
            {
                string aiGeneratedContent = await apiHandler.SendRequestToOpenAI(fileContent, prompt);
                if (!string.IsNullOrEmpty(aiGeneratedContent))
                {
                    // Replace typing message with the actual response
                    ReplaceLastMessage("ReadMeDaddy: \n" + aiGeneratedContent);
                    copyButton.Enabled = true; // Enable the Update button
                }
                else
                {
                    MessageBox.Show("Received empty response from API.", "API Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ReplaceLastMessage("ReadMeDaddy: \nError: Received empty response from API.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing request: " + ex.Message, "Processing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ReplaceLastMessage("ReadMeDaddy: \nError: " + ex.Message);
                copyButton.Enabled = false;
            }
        }


        private void ReplaceLastMessage(string newMessage)
        {
            string typingMessage = "ReadMeDaddy is typing...";
            int typingMessageIndex = richTextBox1.Text.LastIndexOf(typingMessage);

            if (typingMessageIndex != -1)
            {
                richTextBox1.Select(typingMessageIndex, typingMessage.Length);
                richTextBox1.SelectedText = newMessage + "\n";
                richTextBox1.SelectionColor = Color.White;
                richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
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

        private void ApplyCustomFormatting(string text, bool isUser)
        {
            // Split the text into lines while preserving newline characters
            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            bool lastLineWasEmpty = false; // Track if the last processed line was empty to add extra space

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
                        richTextBox1.AppendText(line.Substring(colonIndex + 1).Trim() + "\n");
                        lastLineWasEmpty = false; // Reset as this is a new section
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        if (!lastLineWasEmpty)
                        {
                            richTextBox1.AppendText("\n"); // Add an extra newline for better readability
                            lastLineWasEmpty = true;
                        }
                    }
                    else
                    {
                        richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
                        richTextBox1.AppendText(line + "\n");
                        lastLineWasEmpty = false; // Reset as this is content
                    }
                }
            }
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

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            using (var apiKeyForm = new Form2())
            {
                if (apiKeyForm.ShowDialog() == DialogResult.OK)
                {
                    var apiKey = apiKeyForm.ApiKey;
                    apiHandler = new ApiHandler(apiKey); // Reinitialize with new user-provided API key
                }
            }
        }



        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}