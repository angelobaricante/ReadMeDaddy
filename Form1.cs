using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using AIFileAssistant;
using Newtonsoft.Json.Linq;

namespace ReadMeDaddy
{
    public partial class Form1 : Form
    {
        private ApiHandler apiHandler;
        private string filePath; 
        private string fileContent; 
        private Form2 form2;

        public Form1()
        {
            InitializeComponent();
            InitializeRichTextBox();
            form2 = new Form2();

            settingsButton.Click += SettingsButton_Click;

            clearButton.Click += ClearButton_Click;
            copyButton.Click += CopyButton_Click;

            var apiKey = LoadApiKey();
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("API key is not configured properly.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close(); 
            }
            apiHandler = new ApiHandler(apiKey);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            if (richTextBox1.TextLength > 0)
            {
                string fullText = richTextBox1.Text;
                int lastIndex = fullText.LastIndexOf("ReadMeDaddy:");

                if (lastIndex != -1)
                {
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
            string apiKey = Environment.GetEnvironmentVariable("API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("API key is not configured properly. Please ensure the setup script has been run.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return apiKey;
        }




        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt", 
                FilterIndex = 1, 
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
                fileLabel.Text = filePath; 
                fileContent = FileOperations.ReadFile(filePath);
                if (fileContent.StartsWith("Error reading file:") || fileContent.StartsWith("Unsupported file format."))
                {
                    MessageBox.Show(fileContent, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    processButton.Enabled = false;
                }
                else
                {
                    processButton.Enabled = true;
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

            AppendTextToChat("You: \n" + prompt, true);

            taskInputTextBox.Clear();  

            await Task.Delay(300);

            AppendTextToChat("ReadMeDaddy is typing...", false);

            try
            {
                string aiGeneratedContent = await apiHandler.SendRequestToOpenAI(fileContent, prompt);
                if (!string.IsNullOrEmpty(aiGeneratedContent))
                {
                    ReplaceLastMessage("ReadMeDaddy: \n" + aiGeneratedContent);
                    copyButton.Enabled = true; 
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

        private bool lastMessageWasUser = true;  

        private void AppendTextToChat(string text, bool isUser)
        {
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;

            if (lastMessageWasUser != isUser)
            {
                richTextBox1.AppendText("\n");
            }

            if (isUser)
            {
                richTextBox1.SelectionColor = Color.FromArgb(26, 188, 156); 
            }
            else
            {
                richTextBox1.SelectionColor = Color.White; 
            }

            richTextBox1.SelectionIndent = isUser ? 0 : 50; 
            ApplyCustomFormatting(text, isUser);

            richTextBox1.ScrollToCaret();  

            lastMessageWasUser = isUser;  
        }

        private void ApplyCustomFormatting(string text, bool isUser)
        {
            
            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            bool lastLineWasEmpty = false; 

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
                        lastLineWasEmpty = false; 
                    }
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        if (!lastLineWasEmpty)
                        {
                            richTextBox1.AppendText("\n"); 
                            lastLineWasEmpty = true;
                        }
                    }
                    else
                    {
                        richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
                        richTextBox1.AppendText(line + "\n");
                        lastLineWasEmpty = false; 
                    }
                }
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void taskInputTextBox_TextChanged(object sender, EventArgs e)
        {

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
            if (form2 == null || form2.IsDisposed)
            {
                form2 = new Form2();
            }
            form2.Show();
        }


        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
    }
}