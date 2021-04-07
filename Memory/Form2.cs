using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using static System.Windows.Forms.ListBox;

namespace Memory
{
    public partial class Form2 : Form
    {
        private static TcpListener tcpLsn;
        private static Socket s;
        private static List<int> selected;
        private static int points = 0;
        private static SelectedObjectCollection si;
        Connect con = new Connect();
        public Form2()
        {
            InitializeComponent();
            con.KomunikatPrzybyl += new Connect.KomunikatEventsHandler(pol_KomunikatPrzybyl);
            listBox1.Items.Add(1); // todo obrazki w to miejsce
            listBox1.Items.Add(2);
            listBox1.Items.Add(1);
            listBox1.Items.Add(2);
            AppendColoredText(richTextBox1, "polaczono", Color.Green);
            AppendColoredText(richTextBox1, "\n", Color.Green);
        }

        public delegate void DodajKolorowyTekst(RichTextBox RichTextBox, string Text, Color kolor);
        private void DodajKolorowyTekstFn(RichTextBox rtb, string tekst, Color kolor)
        {
            var StartIndex = rtb.TextLength;
            rtb.AppendText(tekst);
            var EndIndex = rtb.TextLength;
            rtb.Select(StartIndex, EndIndex - StartIndex);
            rtb.SelectionColor = kolor;
        }
        private void AppendColoredText(RichTextBox RTB, string Text, Color kolor)
        {
            if (RTB.InvokeRequired)
            {
                RTB.Invoke(new DodajKolorowyTekst(DodajKolorowyTekstFn), RTB, Text, kolor);
            }
            else
            {
                DodajKolorowyTekstFn(RTB, Text, kolor);
            }
        }

        void pol_KomunikatPrzybyl(object sender, KomunikatEventArgs e)
        {
            AppendColoredText(richTextBox1, "[" + e.kom.enemyName.ToString() + "] ", Color.Blue);
            AppendColoredText(richTextBox1, "\n", Color.Green);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            si = listBox1.SelectedItems;
            selected = new List<int>();
            string r = "";
            foreach (var item in si)
            {
                int singleCustomer = (int)item;
                selected.Add(singleCustomer);
                r += singleCustomer.ToString();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            con.odlacz();
            label3.Visible = true;
            MessageBox.Show("Rozłączono. Gra skończona", "Koniec", MessageBoxButtons.OK);

            this.Hide();
            Form1 f1 = new Form1();
            f1.ShowDialog();
            //this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(selected.Count == 2)
            {
                if (selected.Contains(1) && !selected.Contains(2))
                {
                    label4.Text = "Brawo, punkt dla Ciebie!";
                    points++;
                    label5.Text = "Wynik: " + points;
                }
                else
                {
                    label4.Text = "Niestety, zła odpowiedź";
                }
            }
            label4.Visible = true;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            con.odlacz();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Komunikat kom = new Komunikat();
            kom.card1 = "1";
            kom.card2 = "2";
            kom.enemyName = "marek";
            kom.enemyRate = 3;
            con.wyslij(kom);
        }
    }
}
