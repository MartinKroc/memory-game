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
using System.IO;

namespace Memory
{
    public partial class Form2 : Form
    {
        private static List<int> selected;
        private static int points = 0;
        private static ImageList imgs;
        private static ImageList imgs2;
        private static string playerAdress;
        private static List<int> imgsIndexOrder = new List<int>();
        private bool isServer = false;
        private bool initSet = true;

        ListView lvTemp = new ListView();
        Connect con;
        public Form2(Connect con, bool isServer)
        {
            InitializeComponent();
            this.con = con;
            this.isServer = isServer;
            con.KomunikatPrzybyl += new Connect.KomunikatEventsHandler(pol_KomunikatPrzybyl);
            con.PolaczenieUstanowione += new Connect.PolaczenieUstanowioneEventsHandler(pol_PolaczenieUstanowione);
            con.PolaczenieZerwane += new Connect.PolaczenieZerwaneEventsHandler(pol_PolaczenieZerwane);
            populate();
            listView1.MultiSelect = true;
            AppendColoredText(richTextBox1, "polaczono", Color.Green);
            AppendColoredText(richTextBox1, "\n", Color.Green);
        }

        private void populate()
        {
            imgs = new ImageList();
            imgs.ImageSize = new Size(100, 100);

            imgs2 = new ImageList();
            imgs2.ImageSize = new Size(100, 100);

            String[] paths = { };
            paths = Directory.GetFiles(@"../../testimg");

            String[] paths2 = { };
            //paths2 = Directory.GetFiles("D:/testimg2");
            paths2 = Directory.GetFiles(@"../../testimg2");

            try
            {
                foreach(String path in paths)
                {
                    imgs.Images.Add(Image.FromFile(path));
                }

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            try
            {
                foreach (String path in paths2)
                {
                    for(int i = 0; i<imgs.Images.Count;i++)
                    {
                        imgs2.Images.Add(Image.FromFile(path));
                    } 
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // Celem testów ustawiono widocznosc obrazków

            /*            listView1.SmallImageList = imgs2;
                        lvTemp.SmallImageList = imgs;*/
            listView1.SmallImageList = imgs;
            lvTemp.SmallImageList = imgs2;
            for (int i = 0; i < imgs2.Images.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.ImageIndex = i;
                listView1.Items.Add(lvi);

                ListViewItem lvis = new ListViewItem();
                lvis.ImageIndex = i;
                listView1.Items.Add(lvis);

                ListViewItem lvi2 = new ListViewItem();
                lvi2.ImageIndex = i;
                lvTemp.Items.Add(lvi2);

                ListViewItem lvis2 = new ListViewItem();
                lvis2.ImageIndex = i;
                lvTemp.Items.Add(lvis2);
            }
            if(isServer)
            {
                Randomize(listView1, lvTemp);
            }
            // Pomysł - każdy z graczy ma własny zestaw pomieszanych obrazków, usuwa je lokalnie, natomiast przekazuje tylko wynik
        }

        private void Randomize(ListView lv, ListView lv2)
        {
            ListView.ListViewItemCollection list = lv.Items;
            ListView.ListViewItemCollection list2 = lv2.Items;
            Random rng = new Random();
            int n = list.Count;
            lv.BeginUpdate();
            lv2.BeginUpdate();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                ListViewItem value1 = (ListViewItem)list[k];
                ListViewItem value2 = (ListViewItem)list[n];
                list[k] = new ListViewItem();
                list[n] = new ListViewItem();
                list[k] = value2;
                list[n] = value1;

                ListViewItem value11 = (ListViewItem)list2[k];
                ListViewItem value22 = (ListViewItem)list2[n];
                list2[k] = new ListViewItem();
                list2[n] = new ListViewItem();
                list2[k] = value22;
                list2[n] = value11;

                imgsIndexOrder.Add(k);

            }
            lv.EndUpdate();
            lv2.EndUpdate();
            lv.Invalidate();
            lv2.Invalidate();
/*            var message = string.Join(Environment.NewLine, imgsIndexOrder);
            MessageBox.Show(message);*/
        }

        private void setIndexesForClient(ListView lv, ListView lv2)
        {
            ListView.ListViewItemCollection list = lv.Items;
            ListView.ListViewItemCollection list2 = lv2.Items;
            //Random rng = new Random();
            int n = list.Count;
            int i = 0;
            lv.BeginUpdate();
            lv2.BeginUpdate();
            while (n > 1)
            {
                n--;
                //int k = rng.Next(n + 1);
                int k = imgsIndexOrder[i];
                i++;
                ListViewItem value1 = (ListViewItem)list[k];
                ListViewItem value2 = (ListViewItem)list[n];
                list[k] = new ListViewItem();
                list[n] = new ListViewItem();
                list[k] = value2;
                list[n] = value1;

                ListViewItem value11 = (ListViewItem)list2[k];
                ListViewItem value22 = (ListViewItem)list2[n];
                list2[k] = new ListViewItem();
                list2[n] = new ListViewItem();
                list2[k] = value22;
                list2[n] = value11;
            }
            lv.EndUpdate();
            lv2.EndUpdate();
            lv.Invalidate();
            lv2.Invalidate();
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 1)
            {
                //var i1 = listView1.SelectedItems[0].SubItems[0].Text;
                //var i2 = listView1.SelectedItems[1].SubItems[0].Text;
                string m;
                string k;
                GameInfo kom = new GameInfo();
                ListViewItem temp = listView1.SelectedItems[0];
                ListViewItem temp2 = listView1.SelectedItems[1];
                int t1 = temp.ImageIndex;
                int t2 = temp2.ImageIndex;
                int k1 = listView1.Items.IndexOf(listView1.SelectedItems[0]);
                int k2 = listView1.Items.IndexOf(listView1.SelectedItems[1]);
                //listView1.Items.Add(lvTemp.Items[t1]);
                //listView1.Items.Add(lvTemp.Items[t2]);
                pictureBox1.Image = imgs.Images[t1];
                pictureBox2.Image = imgs.Images[t2];
                if (t1 == t2)
                {
                    MessageBox.Show("Brawo! trafienie");
                    kom.matched = true;
                    m = "tak";
                    kom.gCard1 = k1;
                    kom.gCard2 = k2;
                    //kom.gIndex = t1;
                    listView1.Items.RemoveAt(k1);
                    listView1.Items.RemoveAt(k2 - 1);
                    points++;
                    label1.Text = points.ToString();
                    label1.Refresh();
                }
                else
                {
                    MessageBox.Show("błędne trafienie");
                    kom.matched = false;
                    m = "nie";
                }
                kom.gameType = "trafienie " + m;
                List<Image> il = new List<Image>();
                il.Add(Image.FromFile("D:/testimg/t1.jpeg"));
                kom.imgs = il;
                con.wyslij(kom);
            }
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

        void pol_KomunikatPrzybyl(object sender, GameInfoEventArgs e)
        {
/*            AppendColoredText(richTextBox1, "Trafiono karty: ", Color.Green);
            AppendColoredText(richTextBox1, e.gi.gCard1.ToString(), Color.Green);
            AppendColoredText(richTextBox1, "\n", Color.Green);*/

            if(e.gi.matched)
            {
                listView1.BeginUpdate();
                lvTemp.BeginUpdate();

                listView1.Items.RemoveAt(e.gi.gCard1);
                listView1.Items.RemoveAt(e.gi.gCard2 -1);

                listView1.EndUpdate();
                lvTemp.EndUpdate();
                listView1.Invalidate();
                lvTemp.Invalidate();
                AppendColoredText(richTextBox1, "Trafiono karty: ", Color.Green);
                AppendColoredText(richTextBox1, e.gi.gCard1.ToString(), Color.Green);
                AppendColoredText(richTextBox1, "\n", Color.Green);
                AppendColoredText(richTextBox1, e.gi.gCard2.ToString(), Color.Green);
                AppendColoredText(richTextBox1, "\n", Color.Green);
            }
            AppendColoredText(richTextBox1, isServer.ToString(), Color.Green);
            AppendColoredText(richTextBox1, "\n", Color.Green);

            // set indexes
            if (!isServer && initSet)
            {
                imgsIndexOrder = e.gi.imagesIndexOrder;
                setIndexesForClient(listView1, lvTemp);
                initSet = false;
            }
        }

        void pol_PolaczenieZerwane(object sender, PolaczenieZerwaneEventArgs e)
        {
            AppendColoredText(richTextBox1, "Połączenie o id: " + e.idPolaczenia + " zerwane" + "\n", Color.Red);
        }

        void pol_PolaczenieUstanowione(object sender, PolaczenieUstanowioneEventArgs e)
        {
            AppendColoredText(richTextBox1, "Połączono z: ", Color.Red);
            AppendColoredText(richTextBox1, e.adres.ToString() + "\n", Color.Blue);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            con.odlacz();
            label3.Visible = true;
            MessageBox.Show("Rozłączono. Gra skończona", "Koniec", MessageBoxButtons.OK);
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;

            listView1.Columns.Add("test", 150);
            listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize);

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
            if (isServer)
            {
                GameInfo kom = new GameInfo();
                kom.gameType = "test";
                kom.imagesIndexOrder = imgsIndexOrder;
                con.wyslij(kom);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
