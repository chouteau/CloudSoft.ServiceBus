using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus
{
	public class InMemoryMessage : IMessage
	{
		public string QueueName { get; set; }
		public string Label { get; set; }
		public string Body { get; set; }
		public bool Recoverable { get; set; }
		public void Dispose()
		{
		}
	}
}
