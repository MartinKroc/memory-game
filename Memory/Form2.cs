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

namespace Memory
{
    public partial class Form2 : Form
    {
        private static TcpListener tcpLsn;
        private static Socket s;
        private static int i;
        private static int j;
        private static int points = 0;
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                tcpLsn = new TcpListener(IPAddress.Parse("127.0.0.1"), 2222);
                tcpLsn.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Socket sckt = tcpLsn.AcceptSocket();
            bool connectRes = polacz();
            if(connectRes == true)
            {
                label2.Visible = true;
                listBox1.Visible = true;
                listBox1.Items.Add("Example pic 1");
                listBox1.Items.Add("Example pic 2");
                listBox1.Items.Add("Example pic 1");
                listBox1.Items.Add("Example pic 2");
            }
            bool open = true;
/*            while (open)
            {
                try
                {
                    Byte[] odebraneBajty = new Byte[100];
                    int ret = sckt.Receive(odebraneBajty, odebraneBajty.Length, 0);
                    string tmp = null;
                    tmp = System.Text.Encoding.ASCII.GetString(odebraneBajty);
                    if (tmp.Length > 0)
                    {
                        Console.WriteLine("Odebrałem komunikat:");
                        Console.WriteLine(tmp);
                        if (tmp.StartsWith("quit"))
                        {
                            open = false;
                            continue;
                        }
                        Console.WriteLine("Napisz coś");
                        string m = Console.ReadLine();
                        if (m == "quit")
                        {
                            open = false;
                        }
                        wyslij(m);
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
*/
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
                Console.WriteLine("Rozlaczono");
            }
        }

        private static bool polacz()
        {
            try
            {
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress hostadd = IPAddress.Parse("127.0.0.1");
                int port = 2223;
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tcpLsn.Stop();
            s.Disconnect(false);
            label3.Visible = true;
            MessageBox.Show("Rozłączono. Gra skończona", "Koniec", MessageBoxButtons.OK);
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            i = listBox1.SelectedIndex;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            j = listBox1.SelectedIndex;
            if(i == 0 && j == 2)
            {
                label4.Text = "Brawo, punkt dla Ciebie!";
                points++;
                label5.Text = "Wynik: " + points;
            }
            else
            {
                label4.Text = "Niestety, zła odpowiedź";
            }
            label4.Visible = true;
        }
    }
}
