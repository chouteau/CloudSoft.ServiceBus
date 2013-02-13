CloudSoft.ServiceBus
====================

Broadcasting service asynchronous messages for later traitemnet   

1 - Get service bus (In memory or Msmq or other)   
`var bus = new ServiceBus.InMemoryServiceBus();`   
   
2 - Register the reader with name     
`bus.RegisterReader("test", typeof(PersonMessageReader));`    
   
3 - Start reading   
`bus.StartReading();`   
   
4 - Create message   
`var person = new Person();`   
`person.FirsName = i.ToString();`   
`person.LastName = Guid.NewGuid().ToString();`   
   
5 - Send message   
`m_Bus.Send("test", person);`   


