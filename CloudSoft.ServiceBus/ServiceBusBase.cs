using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;

namespace CloudSoft.ServiceBus
{
	public abstract class ServiceBusBase : IServiceBus, IDisposable
	{
		private System.Collections.Concurrent.ConcurrentDictionary<string,IMessageQueue> m_QueueList;
		private System.Collections.Concurrent.ConcurrentDictionary<string, Type> m_ReaderList;
		private List<IMessageReader> m_Readers;

		private Queue<Action> m_Queue;
		private ManualResetEvent m_NewMessage = new ManualResetEvent(false);
		private ManualResetEvent m_Terminate = new ManualResetEvent(false);
		private bool m_Terminated = false;
		private Thread m_SendThread;

		public ServiceBusBase()
		{
			m_QueueList = new System.Collections.Concurrent.ConcurrentDictionary<string, IMessageQueue>();
			m_ReaderList = new System.Collections.Concurrent.ConcurrentDictionary<string, Type>();
			m_Readers = new List<IMessageReader>();
		}

		#region IServiceBus Members

		public void Send(string queueName, object body, string label = null)
		{
			if (m_Queue == null)
			{
				InitializeQueueSenderThread();
			}
			lock(m_Queue)
			{
				m_Queue.Enqueue(() =>
					{
						string serializedBody = null;
						try
						{
							serializedBody = Serialize(body);
						}
						catch (Exception ex)
						{
							ex.Data.Add("failed object serialization", body.ToString());
							ex.Data.Add("QueueName", queueName);
							ex.Data.Add("Label", label);
							throw;
						}

						var mq = GetQueue(queueName);
						var m = CreateMessage();
						m.Label = label ?? Guid.NewGuid().ToString();
						m.Body = serializedBody;
						mq.Send(m);
					});
				m_NewMessage.Set();
			}
		}

		public void RegisterFromConfig(string configFileName = null)
		{
			string sectionName = "cloudSoft/serviceBus";
			Configuration.ServiceBusConfigurationSection section = null;
			if (configFileName == null) // Default file configuration
			{
				section = System.Configuration.ConfigurationManager.GetSection(sectionName) as Configuration.ServiceBusConfigurationSection;
			}
			else
			{
				var map = new ExeConfigurationFileMap();
				map.ExeConfigFilename = configFileName;
				var config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
				section = config.GetSection(sectionName) as Configuration.ServiceBusConfigurationSection;
			}

			foreach (Configuration.ServiceBusQueueReaderConfigurationElement item in section.QueueReaders)
			{
				var type = Type.GetType(item.Type);
				if (type == null)
				{
					GlobalConfiguration.Configuration.Logger.Warn("Type {0} reader for servicebus does not exists", item.Type);
					continue;
				}
				RegisterReader(item.QueueName, type);
			}
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
			foreach (var item in m_Readers)
			{
				item.Stop();
				item.Dispose();
			}
			m_Readers.Clear();

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
			using (var textWriter = new System.IO.StringWriter(sb))
			{
				using (var jsonWriter = new Newtonsoft.Json.JsonTextWriter(textWriter))
				{
					jsonSerializer.Serialize(jsonWriter, body);
				}
			}
			var result = sb.ToString();
			return result;
		}

		void SendInQueue()
		{
			while (!m_Terminated)
			{
				var waitHandles = new WaitHandle[] { m_Terminate, m_NewMessage };
				int result = ManualResetEvent.WaitAny(waitHandles, 60 * 1000, true);
				if (result == 0)
				{
					m_Terminated = true;
					break;
				}
				m_NewMessage.Reset();

				if (m_Queue.Count == 0)
				{
					continue;
				}
				// Enqueue
				Queue<Action> queueCopy;
				lock (m_Queue)
				{
					queueCopy = new Queue<Action>(m_Queue);
					m_Queue.Clear();
				}

				foreach (var send in queueCopy)
				{
					try
					{
						send();
					}
					catch(Exception ex)
					{
						GlobalConfiguration.Configuration.Logger.Error(ex);
					}
				}
			}
		}

		private void InitializeQueueSenderThread()
		{
			m_Queue = new Queue<Action>();
			m_SendThread = new Thread(new ThreadStart(SendInQueue));
			m_SendThread.IsBackground = true;
			m_SendThread.Start();
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (m_Queue != null)
			{
				m_Queue.Clear();
			}
			m_Terminated = true;
			if (m_Terminate != null)
			{
				m_Terminate.Set();
			}
			if (m_SendThread != null)
			{
				if (m_SendThread != null 
					&& !m_SendThread.Join(TimeSpan.FromSeconds(5)))
				{
					m_SendThread.Abort();
				}
			}
		}

		#endregion
	}
}
