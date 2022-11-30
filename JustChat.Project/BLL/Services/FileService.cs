using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using JustChat.BLL.Interfaces;
using JustChat.DAL.Entities;
using JustChat.DAL.Models.Settings;
using JustChat.DAL.ViewModel;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace JustChat.BLL.Services
{
    public class FileService : IFileService
    {
        private static IAmazonS3 _amazonS3;
        private IMongoCollection<MetaFile> _metaFiles;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
        public FileService(
            IAmazonS3 amazonS3,
            IMongoDBSettings mongoDBSettings,
            IMongoClient mongoClient)
        {
            _amazonS3 = amazonS3;
            var mongoDB = mongoClient.GetDatabase(mongoDBSettings.DatabaseName);
            _metaFiles = mongoDB.GetCollection<MetaFile>(mongoDBSettings.CollectionName);
        }

        private static async Task AddExampleLifecycleConfigAsync(IAmazonS3 client, LifecycleConfiguration configuration, string bucketName)
        {

            PutLifecycleConfigurationRequest request = new PutLifecycleConfigurationRequest
            {
                BucketName = bucketName,
                Configuration = configuration
            };
            await client.PutLifecycleConfigurationAsync(request);
        }

        public async Task CreateBucketAsync(string bucketName)
        {

            try
            {
                if (!(await AmazonS3Util.DoesS3BucketExistAsync(_amazonS3, bucketName)))
                {
                    var putBucketRequest = new PutBucketRequest
                    {
                        BucketName = bucketName,
                        UseClientRegion = true
                    };

                    PutBucketResponse putBucketResponse = await _amazonS3.PutBucketAsync(putBucketRequest);
                }

            }

            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
        }

        public async Task<List<S3Bucket>> GetAllBucketsAsync()
        {
            try
            {
                var response = await _amazonS3.ListBucketsAsync();

                return response.Buckets;

            }
            catch (AmazonS3Exception e)
            {
                throw new Exception($"Error encountered on server. Message:'{e.Message}' when writing an object");

            }
            catch (Exception e)
            {
                throw new Exception($"Error encountered on server. Message:'{e.Message}' when writing an object");

            }

        }

        public async Task<MetaFile> PostFileAsync( MessageRequest message )
        {
            var bucketName = "justbucket";
            if (!(await AmazonS3Util.DoesS3BucketExistAsync(_amazonS3, bucketName)))
            {
                var putBucketRequest = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };

                PutBucketResponse putBucketResponse = await _amazonS3.PutBucketAsync(putBucketRequest);
            }
            try
            {
                var KEY = $"{Guid.NewGuid().ToString() + message.Image.FileName}";
                await using var newMemoryStream = new MemoryStream();
                await message.Image.CopyToAsync(newMemoryStream);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = KEY,
                    BucketName = bucketName,
                    CannedACL = S3CannedACL.PublicRead
                };

                var fileTransferUtility = new TransferUtility(_amazonS3);
                await fileTransferUtility.UploadAsync(uploadRequest);


                var res = new MetaFile
                {
                    MessageId = message.MessageId,
                    Titile = KEY,
                    PublishDate = message.PublishDate,
                    UserName = message.UserName,
                };

                await _metaFiles.InsertOneAsync(res);

                return res;

            }
            catch (AmazonS3Exception e)
            {
                throw new Exception($"Error encountered on server. Message:'{e.Message}' when writing an object");

            }
            catch (Exception e)
            {
                throw new Exception($"Error encountered on server. Message:'{e.Message}' when writing an object");

            }

        }

        public async Task<Stream> GetFileAsync( string bucketName, string objectKey)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                using var response = await _amazonS3.GetObjectAsync(request);
                var responseStream = response.ResponseStream;

                return responseStream;

            }
            catch (AmazonS3Exception e)
            {
                throw new Exception($"Error encountered on server. Message:'{e.Message}' when writing an object");
            }
            catch (Exception e)
            {
                throw new Exception($"Error encountered on server. Message:'{e.Message}' when writing an object");
            }

        }

        public async Task<List<MetaFile>> GetAllFilesAsync()
        {
            try
            {
                return (await _metaFiles.FindAsync(meta => true)).ToList();
                //var response = await _amazonS3.ListObjectsAsync(bucketName);

                //return response.S3Objects;

            }
            catch (AmazonS3Exception e)
            {
                throw new Exception($"Error encountered on server. Message:'{e.Message}' when writing an object");
            }
            catch (Exception e)
            {
                throw new Exception($"Error encountered on server. Message:'{e.Message}' when writing an object");
            }
        }
    }
}
