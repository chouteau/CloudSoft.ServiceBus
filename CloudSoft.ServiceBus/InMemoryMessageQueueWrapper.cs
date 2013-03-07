using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CloudSoft.ServiceBus
{
	public class InMemoryMessageQueueWrapper : IMessageQueue
	{
		private System.Collections.Queue m_Queue;
		private ManualResetEvent m_Event;

		public InMemoryMessageQueueWrapper(System.Collections.Queue queue)
		{
			m_Queue = queue;
			m_Event = new ManualResetEvent(false);
		}

		#region IMessageQueue Members

		public IAsyncResult BeginReceive()
		{
			return new MessageEnqueuedAsyncResult(m_Event);
		}

		public T EndReceive<T>(IAsyncResult r)
		{
			var message = m_Queue.Dequeue();
			var wrapped = message as IMessage;
			return (T)wrapped.Body;
		}

		public void Reset()
		{
			if (m_Queue.Count == 0)
			{
				m_Event.Reset();
			}
		}

		public void Send(IMessage message)
		{
			lock (m_Queue)
			{
				m_Queue.Enqueue(message);
			}
			m_Event.Set();
		}

		#endregion

	}
}
