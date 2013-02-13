using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus
{
	public class MSMQMessageWrapper : IMessage
	{
		private System.Messaging.Message m_Message; 

		public MSMQMessageWrapper(System.Messaging.Message message)
		{
			m_Message = message;
		}

		#region IMessage Members

		public string QueueName { get; set; }

		public string Label
		{
			get
			{
				return m_Message.Label;
			}
			set
			{
				m_Message.Label = value;
			}
		}

		public string Body
		{
			get
			{
				return m_Message.Body as string;
			}
			set
			{
				m_Message.Body = value;
			}
		}

		public bool Recoverable
		{
			get
			{
				return m_Message.Recoverable;
			}
			set
			{
				m_Message.Recoverable = value;
			}
		}

		#endregion

		public void Dispose()
		{
			m_Message.Dispose();
		}
	}
}
