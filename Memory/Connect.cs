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
        public string tresc;
        public bool wazna;
        public string nadawca;
        public DateTime czasNadania;
        public DateTime czasOdbioru;
    }
    class KomunikatEventArgs : EventArgs
    {
        public Komunikat kom;
        public long idPolaczenia;
        public KomunikatEventArgs()
        {
            kom.czasNadania = DateTime.Now;
            kom.nadawca = "";
            kom.tresc = "";
            kom.wazna = true;
        }
    }
    [Serializable]
    public struct GameInfo
    {
        public List<Card> cardsState;
        public string gameType;
    }
    [Serializable]
    public class Card
    {
        public int key;
        public string image;
        public Card(int key, string image)
        {
            this.key = key;
            this.image = image;
        }
    }

    public class GameInfoEventArgs : EventArgs
    {
        public GameInfo gi;
        public long connId;
        public List<Card> l;
        public Card c1;
        public Card c2;
        public Card c3;
        public Card c4;
        public GameInfoEventArgs()
        {
            l = new List<Card>();
            c1 = new Card(1, "i1");
            c2 = new Card(2, "i2");
            c3 = new Card(3, "i3");
            c4 = new Card(4, "i4");
            l.Add(c1);
            l.Add(c2);
            l.Add(c3);
            l.Add(c4);
            gi.cardsState = l;
            gi.gameType = "default";

        }
    }
    public class PolaczenieZerwaneEventArgs : EventArgs
    {
        public long idPolaczenia;
        public PolaczenieZerwaneEventArgs(long id_pol)
        {
            idPolaczenia = id_pol;
        }
    }
    public class PolaczenieUstanowioneEventArgs : EventArgs
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
    public class Connect
    {
        public delegate void KomunikatEventsHandler(object sender, GameInfoEventArgs e);
        //public delegate void KomunikatEventsHandler(object sender, KomunikatEventArgs e);
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

        public bool wyslij(GameInfo kom)
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

/*        public bool wyslijInit(InitInfo kom)
        {
            foreach (Klient kli in clientsList.Values)
            {
                if (kli.tcpKlient.Connected)
                {
                    bf.Serialize(kli.tcpKlient.GetStream(), kom);
                }
            }
            return true;
        }*/

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
                    kli.tcpKlient = tcpLsn.AcceptTcpClientAbortable();
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
                        GameInfo odebranyKom = (GameInfo)bf.Deserialize(tcpclient.GetStream());
                        //Komunikat odebranyKom = (Komunikat)bf.Deserialize(tcpclient.GetStream());
                        GameInfoEventArgs arg = new GameInfoEventArgs();
                        //KomunikatEventArgs arg = new KomunikatEventArgs();
                        arg.gi = odebranyKom;
                        arg.connId = realId;
                        //arg.kom = odebranyKom;
                        //arg.kom.czasOdbioru = DateTime.Now;
                        //arg.idPolaczenia = realId;
                        Console.WriteLine(odebranyKom);
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
