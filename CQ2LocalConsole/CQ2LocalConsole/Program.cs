using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RobotClientProcess;

namespace CQ2LocalConsole
{
    class Program : IRobotClient
    {
        static void Main(string[] args)
        {
            RobotClient.Connect(new Program(), "127.0.0.1", 60604, 1361778219);
            while (true)
            {
                string msg = Console.ReadLine();
                RobotClient.SendMessage(msg);
            }
        }

        public void Connect()
        {
            Console.WriteLine("连接上服务器了！");
        }

        public void DisConnect()
        {
            Console.WriteLine("服务器断开了！");
        }

        public void ReceiveMessage(string msg)
        {
            Console.WriteLine($"服务器的消息：{msg}");
        }
    }
}
