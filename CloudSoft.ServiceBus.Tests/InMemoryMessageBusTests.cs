using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus.Tests
{
	public class InMemoryMessageBusTests
	{
		private ServiceBus.IServiceBus m_Bus;

		public InMemoryMessageBusTests()
		{
			m_Bus = new ServiceBus.InMemoryServiceBus();
			m_Bus.RegisterReadersFromConfig();
			m_Bus.StartReading();
		}

		public void Add_Message()
		{
			for (int i = 0; i < 100; i++)
			{
				var person = new Person();
				person.FirsName = i.ToString();
				person.LastName = Guid.NewGuid().ToString();

				m_Bus.Send("test", person);
			}

			Console.WriteLine("---");
		}
	}
}
