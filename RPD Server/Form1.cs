using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPD_Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            new Form2(int.Parse(txtPort.Text)).Show();
            btnListen.Enabled = false;
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            new Form3(int.Parse(txtPort.Text)).Show();
            btnListen.Enabled = false;
        }
    }
}
