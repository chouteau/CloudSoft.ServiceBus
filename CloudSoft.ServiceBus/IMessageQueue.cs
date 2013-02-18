using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CloudSoft.ServiceBus
{
	public interface IMessageQueue
	{
		IAsyncResult BeginReceive();
		IMessage EndReceive(IAsyncResult result);
		void Reset();
		void Send(IMessage message);
		void SendAsync(IMessage message);
	}
}
