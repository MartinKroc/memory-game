using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Memory
{
    [Serializable]
    public struct Komunikat
    {
        public string card1;
        public string card2;
        public string enemyName;
        public int enemyRate;
    }
    public struct InitInfo
    {
        public int setNr;
        public string gameType;
        public string dif;
    }

    class KomunikatEventArgs : EventArgs
    {
        public Komunikat kom;
        public long idPolaczenia;
        public KomunikatEventArgs()
        {
            kom.card1 = "";
            kom.card2 = "";
            kom.enemyName = "";
            kom.enemyRate = 0;
        }
    }
    class PolaczenieZerwaneEventArgs : EventArgs
    {
        public long idPolaczenia;
        public PolaczenieZerwaneEventArgs(long id_pol)
        {
            idPolaczenia = id_pol;
        }
    }
    class PolaczenieUstanowioneEventArgs : EventArgs
    {
        public long idPolaczenia;
        public string adres;
        public PolaczenieUstanowioneEventArgs(long id_pol, string _adres)
        {
            idPolaczenia = id_pol;
            adres = _adres;
        }
    }
    class Klient
    {
        public Thread watek;
        public TcpClient tcpKlient;
    }
    class Connect
    {
        public delegate void KomunikatEventsHandler(object sender, KomunikatEventArgs e);
        public event KomunikatEventsHandler KomunikatPrzybyl;

        public delegate void PolaczenieZerwaneEventsHandler(object sender, PolaczenieZerwaneEventArgs e);
        public event PolaczenieZerwaneEventsHandler PolaczenieZerwane;

        public delegate void PolaczenieUstanowioneEventsHandler(object sender, PolaczenieUstanowioneEventArgs e);
        public event PolaczenieUstanowioneEventsHandler PolaczenieUstanowione;

        private int MaxClients = 20;
        private static long connectId = 1;

        private TcpListener tcpLsn;
        private Thread serverThread;

        private Hashtable clientsList = new Hashtable();
        private BinaryFormatter bf = new BinaryFormatter();

        public bool startSerwer(string adres, int port)
        {
            tcpLsn = new TcpListener(IPAddress.Parse(adres), port);
            tcpLsn.Start();
            serverThread = new Thread(new ThreadStart(watekCzekajNaKlientow));
            serverThread.Name = "wątek serwera czekający na klientów id: " + 0;
            serverThread.IsBackground = false;
            serverThread.Start();
            return true;
        }

        public bool startKlient(string adres, int port)
        {
            Klient kli = new Klient();
            IPAddress hostadd = IPAddress.Parse(adres);
            IPEndPoint EPhost = new IPEndPoint(hostadd, port);
            kli.tcpKlient = new TcpClient();
            try
            {
                kli.tcpKlient.Connect(EPhost);
                if (kli.tcpKlient.Client.Connected)
                {
                    kli.watek = new Thread(new ParameterizedThreadStart(watekCzytajZSocketa));
                    kli.watek.Name = "Wątek kllienta czytający z socketa id: 0";
                    clientsList.Add(0L, kli);
                    kli.watek.Start(0L);
                }
                else return false;
            }
            catch (Exception e1)
            {
                Console.WriteLine("Siakiś błąd: " + e1.Message);
                return false;
            }
            return true;
        }

        public bool wyslij(Komunikat kom)
        {
            foreach (Klient kli in clientsList.Values)
            {
                if (kli.tcpKlient.Connected)
                {
                    bf.Serialize(kli.tcpKlient.GetStream(), kom);
                }
            }
            return true;
        }

        public bool wyslijInit(InitInfo kom)
        {
            foreach (Klient kli in clientsList.Values)
            {
                if (kli.tcpKlient.Connected)
                {
                    bf.Serialize(kli.tcpKlient.GetStream(), kom);
                }
            }
            return true;
        }

        public void odlacz()
        {
            lock (clientsList)
            {
                foreach (Klient kli in clientsList.Values)
                {
                    kli.tcpKlient.Client.Disconnect(false);
                    kli.tcpKlient.Close();
                    kli.watek.Abort();
                }
                clientsList.Clear();
                if (serverThread != null)
                    serverThread.Abort();
                if (tcpLsn != null)
                    tcpLsn.Server.Close();
            }
        }

        public void watekCzekajNaKlientow()
        {
            try
            {
                while (true)
                {
                    Klient kli = new Klient();                  
                    kli.tcpKlient = tcpLsn.AcceptTcpClient();
                    lock (clientsList)
                    {
                        if (connectId < long.MaxValue - 1)
                            Interlocked.Increment(ref connectId);
                        else
                            connectId = 1;
                        Console.WriteLine("połączono z: " + kli.tcpKlient.Client.RemoteEndPoint.ToString());
                        if (clientsList.Count < MaxClients)
                        {
                            while (clientsList.Contains(connectId))
                            {
                                Interlocked.Increment(ref connectId);
                            }
                            kli.watek = new Thread(new ParameterizedThreadStart(watekCzytajZSocketa));
                            kli.watek.Name = "wątek czytający z socketa id: " + connectId.ToString();
                            clientsList.Add(connectId, kli);
                            kli.watek.Start(connectId);
                            PolaczenieUstanowioneEventArgs arg = new PolaczenieUstanowioneEventArgs(connectId, kli.tcpKlient.Client.RemoteEndPoint.ToString());
                            if (PolaczenieUstanowione != null)
                                PolaczenieUstanowione(this, arg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wątek czekający na klientów dostał wyjątkiem: " + ex.Message);
            }
        }

        public void watekCzytajZSocketa(object id)
        {

            long realId = (long)id;
            TcpClient tcpclient = ((Klient)clientsList[realId]).tcpKlient;
            while (true)
            {
                if (tcpclient.Connected)
                {
                    try
                    {
                        Komunikat odebranyKom = (Komunikat)bf.Deserialize(tcpclient.GetStream());
                        KomunikatEventArgs arg = new KomunikatEventArgs();
                        arg.kom = odebranyKom;
                        arg.idPolaczenia = realId;
                        if (KomunikatPrzybyl != null)
                        {
                            KomunikatPrzybyl(this, arg);
                        }
                    }
                    catch (SerializationException)
                    {
                        break;
                    }
                    catch (Exception)
                    {
                        if (!tcpclient.Connected)
                        {
                            break;
                        }
                    }
                }
            }
            lock (clientsList)
            {
                clientsList.Remove(realId);
            }
            PolaczenieZerwaneEventArgs arg2 = new PolaczenieZerwaneEventArgs(realId);
            if (PolaczenieZerwane != null)
            {
                PolaczenieZerwane(this, arg2);
            }
        }
    }

}
