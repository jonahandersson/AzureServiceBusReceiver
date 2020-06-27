using Microsoft.Azure.ServiceBus;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusReceiver
{
    class Program
    {
        //Constant connection credentials to be moved to secured file 
        const string ServiceBusConnectionString = "Endpoint=sb://az-devjonah.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=pxtosfZg/4Ad7hp7Lw2ihQpPShJ3D7zz+Xbfv8CCwGA=";
        const string QueueName = "devjonahtestqueue1";
        static IQueueClient queueClient; 
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            RegisterOnMsgHandlerAndReceiveMsgs();
            Console.ReadKey();          
            await queueClient.CloseAsync();
            Console.WriteLine("Finished receiving messages on the queue!");
        }

        static void RegisterOnMsgHandlerAndReceiveMsgs()
        {
            //Options for handler
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                //De-queue one msg at a time
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }
                
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceived)
        {
            Console.WriteLine($"Message handler encountered a problem and exception: {exceptionReceived.Exception}");
            var context = exceptionReceived.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting: ");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- EntityPath: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;

        }
        /// <summary>
        ///  Process messages received from the queue
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        static async Task ProcessMessagesAsync(Message msg, CancellationToken token)
        {
            Console.WriteLine($"Received message: SequenceNumber {msg.SystemProperties.SequenceNumber}");
            await queueClient.CompleteAsync(msg.SystemProperties.LockToken);
        }
    }
}
