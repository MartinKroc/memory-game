using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Memory
{
    public partial class Form5 : Form
    {
        public bool serverCreator;
        public Form5()
        {
            InitializeComponent();
            serverCreator = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serverCreator = true;
            this.Hide();
            Form3 f3 = new Form3(this.serverCreator);
            f3.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form3 f3 = new Form3(this.serverCreator);
            f3.ShowDialog();
            this.Close();
        }
    }
}
