using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Memory
{
    public partial class Form3 : Form
    {
        Connect con = new Connect();
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            con.odlacz();
            this.Hide();
            Form1 f1 = new Form1();
            f1.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string gameType = "";
            string dif = "";
            string t = "";

            gameType = GetGameType(radioButton1);
            gameType = GetGameType(radioButton2);

            dif = GetDif(radioButton3);
            dif = GetDif(radioButton4);
            dif = GetDif(radioButton5);
            dif = GetDif(radioButton6);

            Komunikat kom = new Komunikat();
            kom.card1 = "1";
            kom.card2 = "2";
            kom.enemyName = "marek";
            kom.enemyRate = 3;
            con.wyslij(kom);

            this.Hide();
            Form2 f2 = new Form2();
            f2.ShowDialog();
            this.Close();
        }

        private string GetGameType(RadioButton rd)
        {
            if(rd.Checked)
            {
                return rd.Text;
            }
            return "none";
        }

        private string GetDif(RadioButton rd)
        {
            if (rd.Checked)
            {
                return rd.Text;
            }
            return "none";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (con.startSerwer(textBox2.Text, int.Parse(textBox1.Text)))
                {
                    label1.Visible = true;
                    button2.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            con.odlacz();
        }
    }
}
