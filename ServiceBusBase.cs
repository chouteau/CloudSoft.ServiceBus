using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.ServiceBus
{
	public abstract class ServiceBusBase : IServiceBus
	{
		private System.Collections.Concurrent.ConcurrentDictionary<string,IMessageQueue> m_QueueList;
		private System.Collections.Concurrent.ConcurrentDictionary<string, Type> m_ReaderList;
		private List<IMessageReader> m_Readers;

		public ServiceBusBase()
		{
			m_QueueList = new System.Collections.Concurrent.ConcurrentDictionary<string, IMessageQueue>();
			m_ReaderList = new System.Collections.Concurrent.ConcurrentDictionary<string, Type>();
			m_Readers = new List<IMessageReader>();
		}

		#region IServiceBus Members

		public void Send(string queueName, object body, string label = null)
		{
			var mq = GetQueue(queueName);
			var m = CreateMessage();
			m.Label = label ?? Guid.NewGuid().ToString();
			m.Body = Serialize(body);
			m.Recoverable = true;
			mq.Send(m);
		}

		public void RegisterReader(string queueName, Type reader)
		{
			if (m_ReaderList.ContainsKey(queueName))
			{
				return;
			}
			m_ReaderList.TryAdd(queueName, reader);
		}

		public void StartReading()
		{
			foreach (var item in m_ReaderList)
			{
				var reader = (IMessageReader) GlobalConfiguration.Configuration.DependencyResolver.GetService(item.Value);
				var queue = GetQueue(item.Key);
				m_Readers.Add(reader);
				reader.Start(queue);
			}
		}

		public void StopReading()
		{
			foreach (var item in m_Readers)
			{
				item.Stop();
				item.Dispose();
			}

			m_Readers.Clear();
		}

		public void PauseReading()
		{
			foreach (var item in m_Readers)
			{
				item.Pause();
			}
		}

		#endregion

		protected abstract IMessage CreateMessage();
		protected abstract IMessageQueue CreateMessageQueue(string queueName);

		private IMessageQueue GetQueue(string queueName)
		{
			IMessageQueue mq = null;
			if (!m_QueueList.ContainsKey(queueName))
			{
				mq = CreateMessageQueue(queueName);
				m_QueueList.TryAdd(queueName, mq);
			}
			else
			{
				mq = m_QueueList[queueName];
			}
			return mq;
		}

		private string Serialize(object body)
		{
			var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
			var sb = new StringBuilder();
			var textWriter = new System.IO.StringWriter(sb);
			var jsonWriter = new Newtonsoft.Json.JsonTextWriter(textWriter);
			jsonSerializer.Serialize(jsonWriter, body);
			var result = sb.ToString();
			return result;
		}
	}
}
