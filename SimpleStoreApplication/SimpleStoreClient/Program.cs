using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using System.ServiceModel;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System.Fabric;
using Common;

namespace SimpleStoreClient
{
    internal static class Program
    {
        private static NetTcpBinding CreateClientConnectionBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
            {
                SendTimeout = TimeSpan.MaxValue,
                ReceiveTimeout = TimeSpan.MaxValue,
                OpenTimeout = TimeSpan.FromSeconds(5),
                CloseTimeout = TimeSpan.FromSeconds(5),
                MaxReceivedMessageSize = 1024 * 1024
            };
            binding.MaxBufferSize = (int)binding.MaxReceivedMessageSize;
            binding.MaxBufferPoolSize = Environment.ProcessorCount * binding.MaxReceivedMessageSize;

            return binding;
        }


        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        static void Main(string[] args)
        {
            Uri ServiceName = new Uri("fabric:/SimpleStoreApplication/ShoppingCartService");
            ServicePartitionResolver serviceResolver = new ServicePartitionResolver(() =>
            new FabricClient());
            NetTcpBinding binding = CreateClientConnectionBinding();
            Client shoppingClient = new Client(new WcfCommunicationClientFactory<IShoppingCartService>(serviceResolver, binding, null), ServiceName);
            shoppingClient.AddItem(new ShoppingCartItem
            {
                ProductName = "XBOX ONE",
                UnitPrice = 329.0, Amount = 2 }).Wait();
            var list = shoppingClient.GetItems().Result;
            foreach (var item in list)
            {
                Console.WriteLine(string.Format("{0}: {1:C2} X {2} = {3:C2}",
                    item.ProductName, item.UnitPrice, item.Amount, item.LineTotal));
            }
            Console.ReadKey();
        }

    }
}
