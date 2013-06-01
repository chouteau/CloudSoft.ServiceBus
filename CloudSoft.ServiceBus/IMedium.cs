using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus
{
	public interface IMedium
	{
		IMessageQueue CreateMessageQueue(string queueName);
		IMessage CreateMessage();
	}
}
