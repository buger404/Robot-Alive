//修改此处命名空间
using io.github.buger404.intallk.Code;
using Native.Csharp.Sdk.Cqp;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;
using Unity.Extension;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;

namespace Native.Csharp.App
{
	/// <summary>
	/// 酷Q应用主入口类
	/// </summary>
	public static class CQMain
	{
		// 关闭消息测试
		public static bool NotTestInputing = false;
		// 机器人看到的消息的群号码
		public static long GroupID = 554272507;
		// 机器人看到的消息的QQ号
		public static long QQID = 1361778219;

		public static IUnityContainer FakeCoolQ = new UnityContainer();
		public static ICQStartup Startup;
		public static IGroupMessage GroupMessage;
		public static CQApi CQApi;
		public static CQLog CQLog;
		public static int msgid;
		public static void Main()
		{
			Native.Csharp.Sdk.Cqp.CQApi.Sender = new TaskReceiver();
			//传递假的container
			Register(FakeCoolQ);
			//注册假的CQ
			CQApi = new CQApi(404233);
			CQLog = new CQLog(404233);
			//取得注册的接口
			Startup = FakeCoolQ.Resolve<ICQStartup>("酷Q启动事件");
			GroupMessage = FakeCoolQ.Resolve<IGroupMessage>("群消息处理");
			//触发假的启动事件
			Startup.CQStartup(null, new CQStartupEventArgs(CQApi, CQLog, 0, 0, "startup", "CQStartup", 0));

			//启动Tcp监听线程
			Thread t = new Thread(new ThreadStart(WebProcess.Listening));
			t.Start();

			//触发假的群聊事件
			if (NotTestInputing) return;
			while (true)
			{
				string content = Console.ReadLine();
				Console.WriteLine($"↓ 你：\n{content}");
				GroupMessage.GroupMessage(null, new CQGroupMessageEventArgs(CQApi, CQLog, 0, 0, "groupmessage", "CQGroupMessage", 0, 0,
					msgid, GroupID, QQID,"",content,false));
				msgid++;
			}

		}

		/// <summary>
		/// 在应用被加载时将调用此方法进行事件注册, 请在此方法里向 <see cref="IUnityContainer"/> 容器中注册需要使用的事件
		/// </summary>
		/// <param name="container">用于注册的 IOC 容器 </param>
		public static void Register(IUnityContainer container)
		{
            container.RegisterType<ICQStartup, Event_Startup>("酷Q启动事件");
            container.RegisterType<IFriendAdd, Event_FriendAdd>("好友已添加事件处理");
            container.RegisterType<IFriendAddRequest, Event_FriendAddRequest>("好友添加请求处理");
            container.RegisterType<IGroupMessage,Event_GroupMessage>("群消息处理");
            container.RegisterType<IPrivateMessage, Event_PrivateMessage>("私聊消息处理");
		}

		public class TaskReceiver : ISendMsg
		{
			public void NewTask(long qq, string msg)
			{
				WebProcess.SendMsg(qq, msg);
			}
		}
	}
}
