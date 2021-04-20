﻿using System;
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
        Connect con;
        public Form2(Connect con)
        {
            InitializeComponent();
            this.con = con;
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
            imgs.ImageSize = new Size(200, 200);

            String[] paths = { };
            paths = Directory.GetFiles("D:/testimg");

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

            listView1.SmallImageList = imgs;
            listView1.Items.Add("t1", 0);
            listView1.Items.Add("t2", 1);
            listView1.Items.Add("t3", 2);
            listView1.Items.Add("t1", 0);
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 1)
            {
                var i1 = listView1.SelectedItems[0].SubItems[0].Text;
                var i2 = listView1.SelectedItems[1].SubItems[0].Text;
                string m;
                string k;
                GameInfo kom = new GameInfo();
                ListViewItem item = listView1.SelectedItems[0];
                if (i1 == i2)
                {
                    MessageBox.Show("Brawo! trafienie");
                    int k1 = listView1.Items.IndexOf(listView1.SelectedItems[0]);
                    int k2 = listView1.Items.IndexOf(listView1.SelectedItems[1]);
                    kom.matched = true;
                    m = "tak";
                    kom.gCard1 = k1;
                    kom.gCard2 = k2;
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
            AppendColoredText(richTextBox1, "Trafiono karty: ", Color.Green);
            AppendColoredText(richTextBox1, e.gi.gCard1.ToString(), Color.Green);
            AppendColoredText(richTextBox1, "\n", Color.Green);

            //imgs.Images.RemoveAt();
            if(e.gi.matched)
            {
                listView1.Items.RemoveAt(e.gi.gCard1);
                listView1.Items.RemoveAt(e.gi.gCard2 -1);
                //listView1.Items.RemoveByKey(e.gi.gCard);
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
            GameInfo kom = new GameInfo();
            kom.gameType = "other";
            //kom.cardsState = l;
            con.wyslij(kom);
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
    }
}
