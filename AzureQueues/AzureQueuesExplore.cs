using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using NFluent;
using System.Threading.Tasks;

namespace AzureQueues
{
    [TestClass]
    public class AzureQueuesExplore
    {

        private static TestContext _context = null;

        private string _accountName = string.Empty;
        private string _storageKey = string.Empty;


        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _context = context;
        }

        [TestInitialize]
        public void Init()
        {
            this._accountName = _context.Properties["AzureStorageAccount"].ToString();

            this._storageKey = _context.Properties["AzureStorageKey"].ToString();
        }

        [TestMethod]
        public async Task Test_Azure_Queues_CreateNewOne()
        {
         
            StorageCredentials credentials = new StorageCredentials(this._accountName, this._storageKey);

            CloudStorageAccount queueAccount = new CloudStorageAccount(credentials, true);

            CloudQueueClient queueClient = queueAccount.CreateCloudQueueClient();

            CloudQueue queue = queueClient.GetQueueReference("jinqueuemessages");

            bool result = await queue.CreateIfNotExistsAsync();

        }

        [TestMethod]
        public async Task Test_Azure_Queues_SendQueueMessage()
        {
            StorageCredentials credentials = new StorageCredentials(this._accountName, this._storageKey);

            CloudStorageAccount queueAccount = new CloudStorageAccount(credentials, true);

            CloudQueueClient queueClient = queueAccount.CreateCloudQueueClient();

            CloudQueue queue = queueClient.GetQueueReference("jinqueuemessages");

            await queue.CreateIfNotExistsAsync();

            await queue.ClearAsync();

            string strMessage = "Hello World";

            CloudQueueMessage message = new CloudQueueMessage(strMessage);

            await queue.AddMessageAsync(message);

            CloudQueueMessage messageResult = await queue.GetMessageAsync();

            Check.That(messageResult.AsString).IsEqualTo(strMessage).And.IsEqualTo(message.AsString);

            Check.That(messageResult.DequeueCount).IsStrictlyGreaterThan(0);

            await queue.DeleteMessageAsync(messageResult);

        }
    }
}
