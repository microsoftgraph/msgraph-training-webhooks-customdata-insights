using System.Configuration;
using System.Threading.Tasks;

namespace Batch
{
    class Program
    {
        static void Main(string[] args)
        {
            RunAsync(args).GetAwaiter().GetResult();
        }

        static async Task RunAsync(string[] args)
        {

            var clientId = ConfigurationManager.AppSettings["ida:clientId"];

            var batchDemo = new BatchDemo();
            await batchDemo.RunAsync(clientId);
            
            System.Console.WriteLine("Press ENTER to continue.");
            System.Console.ReadLine();
        }
    }
}
