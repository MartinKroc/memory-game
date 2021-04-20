using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        public Form3(bool sc)
        {
            InitializeComponent();
            if (sc)
            {
                button5.Visible = false;
            }
            else
            {
                button4.Visible = false;
                button3.Visible = false;
                groupBox1.Visible = false;
                groupBox2.Visible = false;
                listBox1.Visible = false;
            }
        }

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
            //this.Hide();
            Form2 f2 = new Form2(this.con);
            f2.ShowDialog();
            //this.Close();
            string gameType = "";
            string dif = "";
            string t = "";

            gameType = GetGameType(radioButton1);
            gameType = GetGameType(radioButton2);

            dif = GetDif(radioButton3);
            dif = GetDif(radioButton4);
            dif = GetDif(radioButton5);
            dif = GetDif(radioButton6);

            GameInfo kom = new GameInfo();
            kom.gameType = "othefgr";
            con.wyslij(kom);


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

                    con.KomunikatPrzybyl += new Connect.KomunikatEventsHandler(pol_KomunikatPrzybyl);
                    con.PolaczenieUstanowione += new Connect.PolaczenieUstanowioneEventsHandler(pol_PolaczenieUstanowione);
                    con.PolaczenieZerwane += new Connect.PolaczenieZerwaneEventsHandler(pol_PolaczenieZerwane);
                }
            }
            catch (Exception ex)
            {
                AppendColoredText(richTextBox1, "Bład połączenia: \n", Color.Red);
                AppendColoredText(richTextBox1, ex.Message + "\n", Color.Red);
            }
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            con.odlacz();
        }

        void pol_PolaczenieZerwane(object sender, PolaczenieZerwaneEventArgs e)
        {
            AppendColoredText(richTextBox1, "Połączenie o id: " + e.idPolaczenia + " zerwane" + "\n", Color.Red);
        }

        void pol_PolaczenieKlientZerwane(object sender, PolaczenieZerwaneEventArgs e)
        {
            AppendColoredText(richTextBox1, "Połączenie klienta o id: " + e.idPolaczenia + " zerwane" + "\n", Color.Red);
            klientOdlaczAsync(this); //wołana metoda bezpiecznej zmiany na formatce (ponieważ ten wątek nie jest właścicielem formatki).
        }

        void pol_PolaczenieUstanowione(object sender, PolaczenieUstanowioneEventArgs e)
        {
            AppendColoredText(richTextBox1, "Połączono z: ", Color.Red);
            AppendColoredText(richTextBox1, e.adres.ToString() + "\n", Color.Blue);
        }

        void pol_KomunikatPrzybyl(object sender, GameInfoEventArgs e)
        {
            AppendColoredText(richTextBox1, e.gi.gameType, Color.Green);
            AppendColoredText(richTextBox1, "\n", Color.Green);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (con.startKlient(textBox2.Text, Int32.Parse(textBox1.Text)))
                {
                    label3.Visible = true;
                    button2.Visible = true;

                    con.KomunikatPrzybyl += new Connect.KomunikatEventsHandler(pol_KomunikatPrzybyl);
                    con.PolaczenieUstanowione += new Connect.PolaczenieUstanowioneEventsHandler(pol_PolaczenieUstanowione);
                    con.PolaczenieZerwane += new Connect.PolaczenieZerwaneEventsHandler(pol_PolaczenieKlientZerwane);
                }
            }
            catch (Exception ex)
            {
                AppendColoredText(richTextBox1, "Błąd podłaczenia: \n", Color.Red);
                AppendColoredText(richTextBox1, ex.Message + "\n", Color.Red);
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            GameInfo kom = new GameInfo();
            //kom.cardsState = l;
            kom.gameType = "other";
            con.wyslij(kom);
        }
    }
}
