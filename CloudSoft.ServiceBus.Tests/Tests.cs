using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus.Tests
{
	public class Tests
	{
		public static void Main()
		{
			var tester = new InMemoryMessageBusTests();
			tester.Add_Message();
			Console.ReadKey();
		}
	}
}
