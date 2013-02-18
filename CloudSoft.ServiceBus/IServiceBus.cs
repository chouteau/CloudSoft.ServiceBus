using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus
{
	public interface IServiceBus
	{
		void RegisterFromConfig(string configFileName = null);
		void RegisterReader(string queueName, Type readerType);
		void StartReading();
		void StopReading();
		void PauseReading();
		void Send(string queueName, object body, string label = null);
	}
}
