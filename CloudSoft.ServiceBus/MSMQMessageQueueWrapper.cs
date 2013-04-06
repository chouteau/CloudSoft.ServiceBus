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

		public override string ToString()
		{
			var result = base.ToString();
			result += string.Format("CanRead:{0}{1}", m_Queue.CanRead, System.Environment.NewLine);
			result += string.Format("CanWrite:{0}{1}", m_Queue.CanWrite, System.Environment.NewLine);
			result += string.Format("FormatName:{0}{1}", m_Queue.FormatName, System.Environment.NewLine);
			result += string.Format("Formatter:{0}{1}", m_Queue.Formatter, System.Environment.NewLine);
			result += string.Format("Label:{0}{1}", m_Queue.Label, System.Environment.NewLine);
			result += string.Format("MachineName:{0}{1}", m_Queue.MachineName, System.Environment.NewLine);
			result += string.Format("QueueName:{0}{1}", m_Queue.QueueName, System.Environment.NewLine);
			return result;
		}
	}
}
