using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus.Tests
{
	public class PersonMessageReader : ServiceBus.MessageReaderBase<Person>
	{
		public override void ProcessMessage(Person message)
		{
			Console.WriteLine(message.FirsName);
		}
	}
}
