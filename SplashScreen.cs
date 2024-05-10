using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReadMeDaddy;

namespace AIFileAssistant
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Start();
            panel2.Width += 15;
            if(panel2.Width>= 370)
            {
                timer1.Stop();
                Form1 form = new Form1();
                form.Show();
                this.Hide();
            }
        }

    }
}
