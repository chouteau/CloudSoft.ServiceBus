using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus.Tests
{
	public class MSMQMessageBusTests
	{
		private ServiceBus.IServiceBus m_Bus;

		public MSMQMessageBusTests()
		{
			m_Bus = new ServiceBus.MSMQServiceBus();
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
