using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadMeDaddy
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            exitButton.Click += ExitButton_Click;
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
    }
}
