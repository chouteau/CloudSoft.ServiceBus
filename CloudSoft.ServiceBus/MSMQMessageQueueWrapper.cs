using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus
{
	public class MSMQMessageQueueWrapper : IMessageQueue
	{
		private System.Messaging.MessageQueue m_Queue;

		public MSMQMessageQueueWrapper(System.Messaging.MessageQueue queue)
		{
			m_Queue = queue;
			m_Queue.Formatter = new MSMQJSonMessageFormatter();
		}

		#region IMessageQueue Members

		public IAsyncResult BeginReceive()
		{
			return m_Queue.BeginReceive();
		}

		public T EndReceive<T>(IAsyncResult result)
		{
			var message = m_Queue.EndReceive(result);
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(message.Body as string);
		}

		public void Reset()
		{

		}

		public void Send(IMessage message)
		{
			m_Queue.Send(message);
		}

		#endregion
	}
}
