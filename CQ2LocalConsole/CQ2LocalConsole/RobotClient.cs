using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RobotClientProcess
{
    public interface IRobotClient
    {
        void ReceiveMessage(string msg);
        void Connect();
        void DisConnect();
    }
    class RobotClient
    {
        public static TcpClient tcp;

        private static IRobotClient re;
        private static long qq;
        public static void Connect(IRobotClient Receiver,string IP,int Port,long QQ)
        {
            re = Receiver;qq = QQ;
            tcp = new TcpClient(IP, Port);
            Thread t = new Thread(new ThreadStart(Receive));
            t.Start();
        }

        public static void SendMessage(string msg)
        {
            NetworkStream nwStream = tcp.GetStream(); 
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            nwStream.Write(buffer, 0, buffer.Length);
        }

        private static void Receive()
        {
            do { } while (!tcp.Connected);
            SendMessage("/**setqq**/" + qq.ToString());
            NetworkStream nwStream = tcp.GetStream();
            do { if (!tcp.Connected) { re.DisConnect(); tcp.Close(); return; } } while (tcp.ReceiveBufferSize <= 0);
            re.Connect();

        chats:

            try
            {
                do { if (!tcp.Connected) { re.DisConnect();tcp.Close(); return; } }  while (tcp.ReceiveBufferSize <= 0);

                byte[] buffer = new byte[tcp.ReceiveBufferSize];
                int bytesRead = nwStream.Read(buffer, 0, tcp.ReceiveBufferSize);
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                re.ReceiveMessage(data);

                goto chats;
            }
            catch
            {
                goto chats;
            }

        }
    }
}
