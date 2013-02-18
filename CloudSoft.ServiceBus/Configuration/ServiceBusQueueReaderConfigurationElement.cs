using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CloudSoft.ServiceBus.Configuration
{
	public class ServiceBusQueueReaderConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("queueName", IsRequired = true)]
		public string QueueName
		{
			get
			{
				return (string)this["queueName"];
			}
			set
			{
				this["queueName"] = value;
			}
		}

		[ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get
			{
				return (string)this["type"];
			}
			set
			{
				this["type"] = value;
			}
		}

	}
}
