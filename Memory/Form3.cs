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
        private static TcpListener tcpLsn;
        private static Socket s;
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
                tcpLsn = new TcpListener(IPAddress.Parse("127.0.0.1"), 2222);
                tcpLsn.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Socket sckt = tcpLsn.AcceptSocket();
            bool connectRes = polacz();
            if (connectRes == true)
            {
                label1.Visible = true;
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
    }
}
