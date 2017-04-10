using System;

namespace SK06.RPC.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var rpcClient = new RpcClient())
            {
                Console.WriteLine(" SK06.RPC.Client Request: fib(30)");
                rpcClient.CallAsync("30", resp =>
                {
                    Console.WriteLine(" SK06.RPC.Client Response: '{0}'", resp);
                });

                Console.WriteLine(" Press [enter] to exit...");
                Console.ReadLine();
            }
        }
    }
}
