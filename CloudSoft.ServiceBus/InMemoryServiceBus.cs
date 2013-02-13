using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus
{
	public class InMemoryServiceBus : ServiceBusBase
	{
		public InMemoryServiceBus()
		{
		}

		protected override IMessageQueue CreateMessageQueue(string queueName)
		{
			var result = new System.Collections.Queue();
			return new InMemoryMessageQueueWrapper(result);
		}

		protected override IMessage CreateMessage()
		{
			return new InMemoryMessage();
		}

	}
}
