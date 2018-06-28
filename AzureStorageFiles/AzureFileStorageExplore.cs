using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.File;
using NFluent;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace AzureStorageFiles
{
    [TestClass]
    public class AzureFileStorageExplore
    {
        private static TestContext _context = null;

        private string _storageAccountName = string.Empty;

        private string _storageKey = string.Empty;

        private StorageCredentials _credentials = null;

        private CloudStorageAccount _storageAccount = null;

        CloudFileClient _cloudFileClient = null;

       [ClassInitialize]
        public static void Init(TestContext context)
        {
            _context = context;
        }

        [TestInitialize]
        public void Init()
        {
            this._storageAccountName =
                  _context.Properties["AzureStorageAccount"].ToString();


            this._storageKey =
                   _context.Properties["AzureStorageKey"].ToString();

            this._credentials =
                          new StorageCredentials(_storageAccountName, _storageKey);

           this._storageAccount = new CloudStorageAccount(this._credentials, true);

            this._cloudFileClient = this._storageAccount.CreateCloudFileClient();
        }

        [TestMethod]
        public async Task Test_Azure_File_Storage_Connect()
        {

            Check.That(this._storageAccountName).IsNotEmpty();

            Check.That(this._storageKey).IsNotEmpty();

            CloudFileShare sharedFile = this._cloudFileClient.GetShareReference("demofileshare01");

            bool result = await sharedFile.ExistsAsync(); //checks whether is exists

            Check.That(result).IsTrue();

        }

        [TestMethod]
        public async Task Test_Azure_File_Storage_Query_Shared_Files()
        {
            Check.That(this._storageAccountName).IsNotEmpty();

            Check.That(this._storageKey).IsNotEmpty();


            List<CloudFileShare> fileShares = new List<CloudFileShare>();

            FileContinuationToken fct = null;

            do
            {
                var result = await this._cloudFileClient.ListSharesSegmentedAsync(fct);

                fct = result.ContinuationToken;

                fileShares.AddRange(result.Results);

            } while (fct != null);


            Check.That(fileShares.Count).IsStrictlyGreaterThan(0);

        }

        [TestMethod]
        public async Task Test_Azure_File_Storage_Root_Directory_FilesAndDirectories()
        {

            Check.That(this._storageAccountName).IsNotEmpty();

            Check.That(this._storageKey).IsNotEmpty();

            CloudFileShare client =   this._cloudFileClient.GetShareReference("demofileshare01");

            CloudFileDirectory rootDirectory = client.GetRootDirectoryReference();

            bool isRootDirectoryExists = await rootDirectory.ExistsAsync();

            FileContinuationToken fct = null;

            List<FileResultSegment> fileResultSegment = new List<FileResultSegment>();

            do
            {
                var result = await rootDirectory.ListFilesAndDirectoriesSegmentedAsync(fct);

                fct = result.ContinuationToken;

                fileResultSegment.Add(result);


            } while (fct != null);

           

        }

        [TestMethod]
        public async Task Test_Azure_File_Storage_Upload()
        {
            CloudFileShare shareReference =
                     this._cloudFileClient.GetShareReference("test232323");

            bool created = await shareReference.CreateIfNotExistsAsync();

            Check.That(created).IsFalse();

            var root = shareReference.GetRootDirectoryReference();

            var file = root.GetFileReference("microsoft-logo.png");

            string filePath = System.IO.Path.Combine( @"C:\Users\jindev\Desktop", file.Name);

            await file.UploadFromFileAsync(filePath);

        }
    }
}
