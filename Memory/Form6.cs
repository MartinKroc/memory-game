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
    public partial class Form6 : Form
    {
        Connect con = new Connect();

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

        private void klientOdlaczAsync(Form fm)
        {
            if (fm.InvokeRequired)
            {
                fm.Invoke(new MethodInvoker(() => { con.odlacz(); }));
            }
            else
                con.odlacz();
        }
        public Form6()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            con.odlacz();
            this.Hide();
            Form1 f1 = new Form1();
            f1.ShowDialog();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (con.startKlient(textBox1.Text, Int32.Parse(textBox2.Text)))
            {
                label3.Visible = true;
                button2.Visible = true;

                //con.KomunikatPrzybyl += new Connect.KomunikatEventsHandler(pol_KomunikatPrzybyl);
                con.PolaczenieUstanowione += new Connect.PolaczenieUstanowioneEventsHandler(pol_PolaczenieUstanowione);
                con.PolaczenieZerwane += new Connect.PolaczenieZerwaneEventsHandler(pol_PolaczenieKlientZerwane);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //this.Hide();
            //Form2 f2 = new Form2();
            //f2.ShowDialog();
            //this.Close();
        }

        private void Form6_FormClosed(object sender, FormClosedEventArgs e)
        {
            con.odlacz();
        }

        void pol_PolaczenieZerwane(object sender, PolaczenieZerwaneEventArgs e)
        {

        }

        void pol_PolaczenieKlientZerwane(object sender, PolaczenieZerwaneEventArgs e)
        {

        }

        void pol_PolaczenieUstanowione(object sender, PolaczenieUstanowioneEventArgs e)
        {

        }

        void pol_KomunikatPrzybyl(object sender, KomunikatEventArgs e)
        {
        }
    }
}
