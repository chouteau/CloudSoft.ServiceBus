using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CloudSoft.ServiceBus
{
	public abstract class MessageReaderBase<T> : IDisposable, IMessageReader
	{
		private ManualResetEvent m_EventStop;
		private bool m_Terminated = false;
		private Thread m_Thread;
		private IMessageQueue m_Queue;

		public MessageReaderBase()
		{
			Logger = GlobalConfiguration.Configuration.Logger;
		}

		protected ILogger Logger { get; set; }

		public virtual void Start(IMessageQueue queue)
		{
			m_EventStop = new ManualResetEvent(false);
			m_Queue = queue;
			m_Thread = new Thread(new ThreadStart(Run));
			m_Thread.Start();
		}

		public virtual void Pause()
		{
			m_Thread.Interrupt();
		}

		public void Terminate()
		{
			m_Terminated = true;
			if (m_EventStop != null)
			{
				m_EventStop.Set();
			}
		}

		public virtual void Stop()
		{
			Terminate();
			// Attendre 5 secondes avant de tuer le process
			if (m_Thread != null && !m_Thread.Join(TimeSpan.FromSeconds(5)))
			{
				m_Thread.Abort();
			}
		}

		public abstract void ProcessMessage(T message);

		private void Run()
		{
			while (!m_Terminated && m_Queue != null)
			{
				IAsyncResult result;
				result = m_Queue.BeginReceive();
				var waitHandles = new WaitHandle[] { m_EventStop, result.AsyncWaitHandle };
				int index = WaitHandle.WaitAny(waitHandles);
				if (index == 0)
				{
					m_Terminated = true;
					break;
				}

				var m = m_Queue.EndReceive(result);
				m_Queue.Reset();
				try
				{
					var message = Deserialize(m.Body as string);
					ProcessMessage(message);
				}
				catch(Exception ex)
				{
					Logger.Error(ex);
				}
			}
		}

		private T Deserialize(string body)
		{
			if (body == null)
			{
				return default(T);
			}
			T result = default(T);
			var jsonSerializer = new Newtonsoft.Json.JsonSerializer();
			var buffer = System.Text.Encoding.UTF8.GetBytes(body);
			using (var sb = new System.IO.MemoryStream(buffer))
			{
				using (var textReader = new System.IO.StreamReader(sb))
				{
					using (var jsonReader = new Newtonsoft.Json.JsonTextReader(textReader))
					{
						result = jsonSerializer.Deserialize<T>(jsonReader);
						jsonReader.Close();
					}
					textReader.Close();
				}
				sb.Close();
			}
			return result;
		}

		#region IDisposable Members

		public virtual void Dispose()
		{
			Terminate();
		}

		#endregion
	}
}
