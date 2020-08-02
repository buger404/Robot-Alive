using Native.Csharp.App;
using Native.Csharp.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Native.Csharp
{
    public class WebProcess
    {
        public static int IPPort = 60604;      // 端口设置

        public struct Client
        {
            public TcpClient Tcp;
            public long QQ;
        }
        public static List<Client> Clients = new List<Client>();
        public static TcpClient ltc;

        public static void SendMsg(long qq,string ret)
        {
            try
            {
                foreach(Client c in Clients)
                {
                    if(c.QQ == qq || qq == 0)
                    {
                        TcpClient tcp = c.Tcp;
                        NetworkStream nwStream = tcp.GetStream();
                        byte[] buffer = Encoding.UTF8.GetBytes(ret);
                        nwStream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch { return; }
        }
        public static void Chating()
        {
            Console.WriteLine("新的QQ连接上了服务器！🔗！");
            TcpClient tc = ltc;long QQ = 0;
            Clients.Add(new Client { Tcp = tc ,QQ = 0});

        chats:
            try
            {

                NetworkStream nwStream;
                try { nwStream = tc.GetStream(); }
                catch { return; }
                do 
                {
                    if (!tc.Connected)
                    {
                        Console.WriteLine($"{QQ}离开了服务器。");
                        tc.Close();
                        Clients.RemoveAt(Clients.FindIndex(m => m.Tcp.Equals(tc)));
                        return;
                    }
                } while (tc.ReceiveBufferSize <= 0);

                byte[] buffer = new byte[tc.ReceiveBufferSize];
                int bytesRead = nwStream.Read(buffer, 0, tc.ReceiveBufferSize);
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (data.StartsWith("/**setqq**/"))
                {
                    int index = Clients.FindIndex(m => m.Tcp.Equals(tc));
                    Client c = Clients[index];
                    c.QQ = long.Parse(data.Split(new string[] { "/**setqq**/" }, StringSplitOptions.None)[1]);
                    QQ = c.QQ;
                    Console.WriteLine($"连接到服务器的QQ：{QQ}");
                    SendMsg(QQ, "/**setok**/");
                }
                else
                {
                    CQMain.GroupMessage.GroupMessage(null, new CQGroupMessageEventArgs(CQMain.CQApi, CQMain.CQLog, 0, 0, "groupmessage", "CQGroupMessage", 0, 0,
                    CQMain.msgid, CQMain.GroupID, QQ, "", data, false));
                    CQMain.msgid++;
                }

                goto chats;
            }
            catch (Exception ex)
            {
                Console.WriteLine("error occured: " + ex.Message + "\n" + ex.StackTrace);
                goto chats;
            }

        }

        public static void Listening()
        {
        lis:
            TcpListener t = new TcpListener(IPAddress.Any, IPPort);
            t.Start();
            ltc = t.AcceptTcpClient();
            Thread th = new Thread(new ThreadStart(Chating));
            th.Start();
            t.Stop();
            goto lis;
        }
    }
}
