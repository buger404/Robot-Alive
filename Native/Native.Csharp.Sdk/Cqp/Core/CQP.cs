using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Native.Csharp.Sdk.Cqp.Expand;
using System.Text;
using Native.Csharp.Sdk.Cqp.Enum;
using Native.Csharp.Sdk.Cqp.Model;
using OpenCover.Framework.Communication;
using Native.Csharp;

namespace OpenCover.Framework.Communication
{
    public interface IMarshalWrapper
    {
        T PtrToStructure<T>(IntPtr pinnedMemory);
        void StructureToPtr<T>(T structure, IntPtr pinnedMemory, bool fDeleteOld);
    }

    public class MarshalWrapper : IMarshalWrapper
    {
        public T PtrToStructure<T>(IntPtr pinnedMemory)
        {
            return (T)Marshal.PtrToStructure(pinnedMemory, typeof(T));
        }

        public void StructureToPtr<T>(T structure, IntPtr pinnedMemory, bool fDeleteOld)
        {
            Marshal.StructureToPtr(structure, pinnedMemory, fDeleteOld);
        }
    }
}

namespace Native.Csharp.Sdk.Cqp.Core
{
    internal static class CQP
    {
        /* 
		 * 官方SDK更新日志:
		 * 
         *  V9 应用机制内的变动（即 V9.x 内的变动）通常**不会**影响已发布的应用，但有可能需要开发者在更新旧版本应用时，对代码进行细微调整。
         *  
         *  V9.25 (2019-10-8)
         *  -----------------
         *  新增 取群信息 Api
         *   * 支持获取群当前人数与人数上限。如果您此前使用其他方式获取，建议迁移至本接口。
         *   
         *  新增 群禁言事件
         *  
         *  V9.23 (2019-9-3)
         *  ----------------
         *  新增 扩展 Cookies 适用范围
         *   * 获取 Cookies 时，填写 Cookies 将要使用的域名，如 api.example.com，可以获得该域名的强登录态 Cookies。
         *     强登录态 Cookies 仅支持部分域名，由于协议差异，酷Q Air 及 酷Q Pro 支持的域名不同。
         *  
		 *	V9.20 (2019-3-3)
		 *	----------------
		 *
		 *	修改 接收语音 Api
		 *	 * 新版 SDK 中，该 Api 将返回本地**绝对**路径（旧版 SDK 是返回相对路径）。
		 *	   若此前用到了该 Api，需要对代码做相应调整。
		 *
		 *	新增 接收图片 Api
		 *	 * 可以使用该 Api，让酷Q直接下载收到的图片，并返回本地**绝对**路径。
		 *	   更新应用时，需要切换至该 Api，在未来获得更好的兼容性。
		 *
		 *	新增 是否支持发送图片() 及 是否支持发送语音() Api
		 *	 * 在开发图片及语音相关的应用时，可用该 Api 处理不支持对应功能的情况（如酷Q Air）。
		 */

        #region --常量--
        private const string DllName = "CQP.dll";
        #endregion

        #region --CqpApi--

        public static string SolveString(IntPtr ip)
        {
            string s = Marshal.PtrToStringAnsi(ip);
            //Marshal.FreeHGlobal(ip);
            return s;
        }

        public static int CQ_sendPrivateMsg (int authCode, long qqId, IntPtr msg)
        {
            string Msg = SolveString(msg);
            Console.WriteLine($"↑ 发给QQ{qqId}：\n{Msg}");
            CQApi.Sender.NewTask(qqId, Msg);
            return 0;
        }

        public static int CQ_sendGroupMsg(int authCode, long groupId, IntPtr msg)
        {
            string Msg = SolveString(msg);
            Console.WriteLine($"↑ 发给群{groupId}：\n{Msg}");
            CQApi.Sender.NewTask(0, Msg);
            return 0;
        }

        public static int CQ_sendDiscussMsg (int authCode, long discussId, IntPtr msg)
        {
            string Msg = SolveString(msg);
            Console.WriteLine($"↑ 发给讨论组{discussId}：\n{Msg}");
            CQApi.Sender.NewTask(0, Msg);
            return 0;
        }

        public static int CQ_deleteMsg (int authCode, long msgId)
        {
            Console.WriteLine($"↑ 撤回了消息{msgId}");
            return 0;
        }

        public static int CQ_sendLikeV2 (int authCode, long qqId, int count)
        {
            Console.WriteLine($"↑ 给{qqId}点了{count}次赞！");
            return 0;
        }

        public static IntPtr CQ_getCookiesV2 (int authCode, IntPtr domain)
        {
            //不知道不知道不返回不返回
            return IntPtr.Zero;
        }

        public static IntPtr CQ_getRecordV2 (int authCode, IntPtr file, IntPtr format)
        {
            //不知道不知道不返回不返回
            return IntPtr.Zero;
        }

        public static int CQ_getCsrfToken (int authCode)
        {
            //不知道不知道不返回不返回
            return 0;
        }

        public static IntPtr CQ_getAppDirectory (int authCode)
        {
            //不知道不知道不返回不返回
            return Marshal.StringToHGlobalAnsi(Environment.CurrentDirectory);
        }

        public static long CQ_getLoginQQ (int authCode)
        {
            //不知道不知道不返回不返回
            return 66666666;
        }

        public static IntPtr CQ_getLoginNick (int authCode)
        {
            //不知道不知道不返回不返回
            return Marshal.StringToHGlobalAnsi("机器人");
        }
    
        public static int CQ_setGroupKick (int authCode, long groupId, long qqId, bool refuses)
        {
            Console.WriteLine($"↑ 踢掉了群{groupId}的{qqId}，并且接收后续申请：{!refuses}");
            return 0;
        }

        public static int CQ_setGroupBan (int authCode, long groupId, long qqId, long time)
        {
            Console.WriteLine($"↑ 禁言了群{groupId}的{qqId}，时长：{time}");
            return 0;
        }

        public static int CQ_setGroupAdmin (int authCode, long groupId, long qqId, bool isSet)
        {
            Console.WriteLine($"↑ 把群{groupId}的{qqId}设置成了管理员");
            return 0;
        }

        public static int CQ_setGroupSpecialTitle (int authCode, long groupId, long qqId, IntPtr title, long durationTime)
        {
            string Title = SolveString(title);
            Console.WriteLine($"↑ 修改了群{groupId}的{qqId}的头衔：{Title}，有效时长：{durationTime}");
            return 0;
        }

        public static int CQ_setGroupWholeBan (int authCode, long groupId, bool isOpen)
        {
            Console.WriteLine($"↑ 开启了群{groupId}的全体禁言：{isOpen}");
            return 0;
        }

        public static int CQ_setGroupAnonymousBan (int authCode, long groupId, IntPtr anonymous, long banTime)
        {
            Console.WriteLine($"↑ 不知道群{groupId}的谁匿名聊天被禁言了：{banTime}");
            return 0;
        }

        public static int CQ_setGroupAnonymous (int authCode, long groupId, bool isOpen)
        {
            Console.WriteLine($"↑ 开启了群{groupId}的匿名：{isOpen}");
            return 0;
        }

        public static int CQ_setGroupCard (int authCode, long groupId, long qqId, IntPtr newCard)
        {
            string card = SolveString(newCard);
            Console.WriteLine($"↑ 修改了群{groupId}的{qqId}的名片：{card}");
            return 0;
        }

        public static int CQ_setGroupLeave (int authCode, long groupId, bool isDisband)
        {
            Console.WriteLine($"↑ 解散了群{groupId}");
            return 0;
        }

        public static int CQ_setDiscussLeave (int authCode, long disscussId)
        {
            Console.WriteLine($"↑ 解散了讨论组{disscussId}");
            return 0;
        }

        public static int CQ_setFriendAddRequest (int authCode, IntPtr identifying, int requestType, IntPtr appendMsg)
        {
            Console.WriteLine($"↑ 不知道想和谁加好友");
            return 0;
        }

        public static int CQ_setGroupAddRequestV2 (int authCode, IntPtr identifying, int requestType, int responseType, IntPtr appendMsg)
        {
            Console.WriteLine($"↑ 不知道想加什么群");
            return 0;
        }

        public static int CQ_addLog (int authCode, int priority, IntPtr type, IntPtr msg)
        {
            string Msg = (string)Marshal.PtrToStructure(msg, typeof(string));
            Console.WriteLine($"日志：{Msg}");
            return 0;
        }


        public static int CQ_setFatal (int authCode, IntPtr errorMsg)
        {
            string Msg = (string)Marshal.PtrToStructure(errorMsg, typeof(string));
            Console.WriteLine($"反馈错误：{Msg}");
            return 0;
        }

        public static IntPtr CQ_getGroupMemberInfoV2 (int authCode, long groudId, long qqId, bool isCache)
        {
            byte[] buff = new byte[255];
            using (BinaryWriter bw = new BinaryWriter(new MemoryStream(buff, true)))
            {
                bw.Write(groudId); //Group
                bw.Write(qqId); //QQ
                bw.Write(qqId.ToString()); //Nick
                bw.Write(qqId.ToString()); //Card
                bw.Write(255); //Sex
                bw.Write(0); //Age
                bw.Write("火星"); //Area
                bw.Write(0);
                bw.Write(0);
                bw.Write("最高"); //等级
                bw.Write(0); //是否管理员
                bw.Write(0); //不良记录
                bw.Write(""); //头衔
                bw.Flush();
            }
            return Marshal.StringToHGlobalAnsi(Convert.ToBase64String(buff));
        }

        public static IntPtr CQ_getGroupMemberList (int authCode, long groupId)
        {
            //不知道不知道不返回不返回
            return IntPtr.Zero;
        }

        public static IntPtr CQ_getGroupList (int authCode)
        {
            //不知道不知道不返回不返回
            return IntPtr.Zero;
        }

        public static IntPtr CQ_getStrangerInfo (int authCode, long qqId, bool notCache)
        {
            byte[] buff = new byte[255];
            using (BinaryWriter bw = new BinaryWriter(new MemoryStream(buff,true)))
            {
                bw.Write(qqId);
                bw.Write(qqId.ToString());
                bw.Write(255);
                bw.Write(0);
            }
            return Marshal.StringToHGlobalAnsi(Convert.ToBase64String(buff));
        }

        public static int CQ_canSendImage (int authCode)
        {
            //假装Air
            return 0;
        }

        public static int CQ_canSendRecord (int authCode)
        {
            //假装Air
            return 0;
        }

        public static IntPtr CQ_getImage (int authCode, IntPtr file)
        {
            //不知道不知道不返回不返回
            return IntPtr.Zero;
        }

        public static IntPtr CQ_getGroupInfo (int authCode, long groupId, bool notCache)
        {
            byte[] buff = new byte[255];
            using (BinaryWriter bw = new BinaryWriter(new MemoryStream(buff, true)))
            {
                bw.Write(groupId); //Number
                bw.Write(groupId.ToString()); //Name
                bw.Write(100); //CurrentMemberCount
                bw.Write(1000); //Max
            }
            return Marshal.StringToHGlobalAnsi(Convert.ToBase64String(buff));
        }

		public static IntPtr CQ_getFriendList (int authCode, bool reserved)
        {
            //不知道不知道不返回不返回
            return IntPtr.Zero;
        }
        #endregion
    }
}
