using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gs_No1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void fitSize()
        {
            this.uiMain1.Location = new Point(0, 0);
            this.uiMain1.Width = this.ClientSize.Width;
            this.uiMain1.Height = this.ClientSize.Height;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            this.fitSize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.fitSize();
        }

    }
}
