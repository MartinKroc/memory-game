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

namespace Klient
{
    public partial class Form1 : Form
    {
        private static TcpListener tcpLsn;
        private static Socket s;
        private static bool open;
        private static List<int> selected;
        private static SelectedObjectCollection si;
        private static int points = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool connectionRes = polacz();
            if (connectionRes == true)
            {
                label2.Visible = true;
                listBox1.Visible = true;
                listBox1.Items.Add(1); // todo obrazki w to miejsce
                listBox1.Items.Add(2);
                listBox1.Items.Add(1);
                listBox1.Items.Add(2);
            }
            try
            {
                tcpLsn = new TcpListener(IPAddress.Parse("127.0.0.1"), 2223);
                tcpLsn.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Socket sckt = tcpLsn.AcceptSocket();    
        }
        private static bool polacz()
        {
            try
            {
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress hostadd = IPAddress.Parse("127.0.0.1");
                int port = 2222;
                IPEndPoint EPhost = new IPEndPoint(hostadd, port);
                s.Connect(EPhost);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private static void wyslij(string wiadomosc)
        {
            if (s != null && s.Connected)
            {
                Byte[] byteData = Encoding.ASCII.GetBytes(wiadomosc.ToCharArray());
                s.Send(byteData, byteData.Length, 0);
            }
            else
            {
                Console.WriteLine("Rozłączono");
            }
        }

        private static void odbierz(Socket sckt)
        {
            string tmp = null;
            Byte[] odebraneBajty = new Byte[100];
            int ret = sckt.Receive(odebraneBajty, odebraneBajty.Length, 0);
            if (ret == 0)
            {
                Console.WriteLine("Połączenie zerwane");
                open = false;
            }
            else
            {
                tmp = System.Text.Encoding.ASCII.GetString(odebraneBajty);
                if (tmp.Length > 0)
                {
                    Console.WriteLine("Odebrałem komunikat:");
                    Console.WriteLine(tmp);
                    if (tmp.StartsWith("quit"))
                    {
                        tcpLsn.Stop();
                        open = false;
                    }
                }
                else
                {
                    if (!sckt.Connected)
                    {
                        open = false;
                    }
                    Console.WriteLine("Błąd odbioru");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tcpLsn.Stop();
            label3.Visible = true;
            MessageBox.Show("Rozłączono. Gra skończona", "Koniec", MessageBoxButtons.OK);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (selected.Count == 2)
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

        private void button3_Click(object sender, EventArgs e)
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
    }
}
