using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Native.Csharp.Sdk.Cqp.Interface
{
    public interface ISendMsg
    {
        void NewTask(long qq, string msg);
    }
}
