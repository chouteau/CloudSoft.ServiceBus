﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus
{
	public interface IMessage : IDisposable
	{
		string QueueName { get; set; }
		string Label { get; set; }
		object Body { get; set; }
		bool Recoverable { get; set; }
	}
}
