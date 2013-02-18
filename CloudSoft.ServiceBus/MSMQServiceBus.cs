using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus
{
	public class MSMQServiceBus : ServiceBusBase
	{
		public MSMQServiceBus()
		{
		}

		protected override IMessageQueue CreateMessageQueue(string queueName)
		{
			string path = System.Configuration.ConfigurationManager.ConnectionStrings[queueName].ConnectionString;
			var  result = new System.Messaging.MessageQueue(path);
			result.QueueName = queueName;
			return new MSMQMessageQueueWrapper(result);
		}

		protected override IMessage CreateMessage()
		{
			var result = new System.Messaging.Message();
			result.Priority = System.Messaging.MessagePriority.Normal;
			result.Recoverable = true;
			return new MSMQMessageWrapper(result);
		}
	}
}
