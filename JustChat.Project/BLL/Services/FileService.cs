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
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Driver;
using System.Text;
using System.Text.Json;

namespace JustChat.BLL.Services
{
    public class FileService : IFileService
    {
        private static IAmazonS3 _amazonS3;
        private IMongoCollection<MetaFile> _metaFiles;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
        private readonly IDistributedCache _cache;

        public FileService(
            IAmazonS3 amazonS3,
            IMongoDBSettings mongoDBSettings,
            IMongoClient mongoClient,
            IDistributedCache cache)
        {
            _amazonS3 = amazonS3;
            var mongoDB = mongoClient.GetDatabase(mongoDBSettings.DatabaseName);
            _metaFiles = mongoDB.GetCollection<MetaFile>(mongoDBSettings.CollectionName);
            _cache = cache;
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
                #region --- Temp Bucket ----
                LifecycleConfiguration newConfiguration = new LifecycleConfiguration
                {
                    Rules = new List<LifecycleRule>
                    {
                        new LifecycleRule
                        {
                            Prefix = "Temp-",
                            Expiration = new LifecycleRuleExpiration {  Days = 1 }
                        },
                    }
                };
                var tempBucketName = "tempbucket" + Guid.NewGuid().ToString();

                var putTempBucketRequest = new PutBucketRequest
                {
                    BucketName = tempBucketName,
                    UseClientRegion = true
                };

                PutBucketResponse putTempBucketResponse = await _amazonS3.PutBucketAsync(putTempBucketRequest);

                var putLifecycleConfigurationTempBucketRequest = new PutLifecycleConfigurationRequest
                {
                    BucketName = tempBucketName,
                    Configuration = newConfiguration,
                };
                await _amazonS3.PutLifecycleConfigurationAsync(putLifecycleConfigurationTempBucketRequest);

                #endregion

                var KEY = $"{Guid.NewGuid().ToString() + message.Image.FileName}";
                await using var newMemoryStream = new MemoryStream();
                await using var newTempMemoryStream = new MemoryStream();
                await message.Image.CopyToAsync(newMemoryStream);
                var checkCounter = 0;
                var cacheKey = Guid.NewGuid().ToString();

                try
                {
                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newTempMemoryStream,
                        Key = KEY,
                        BucketName = tempBucketName,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    var fileTransferUtility = new TransferUtility(_amazonS3);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                    checkCounter++;
                }
                catch (Exception)
                {
                    throw;
                }

                if(checkCounter == 1)
                {
                    var res = new MetaFile
                    {
                        MessageId = message.MessageId,
                        Titile = KEY,
                        PublishDate = message.PublishDate,
                        UserName = message.UserName,
                    };

                    string cachedDataString = JsonSerializer.Serialize(res);
                    var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);

                    DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(3));

                    await _cache.SetAsync(cacheKey, dataToCache, options);
                    checkCounter++;
                }

                if(checkCounter == 2)
                {
                    await _cache.RemoveAsync(cacheKey);
                    await DeleteBucketAsync(_amazonS3, tempBucketName);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = KEY,
                        BucketName = bucketName,
                        CannedACL = S3CannedACL.PublicRead
                    };
                    var res = new MetaFile
                    {
                        MessageId = message.MessageId,
                        Titile = KEY,
                        PublishDate = message.PublishDate,
                        UserName = message.UserName,
                    };
                    var fileTransferUtility = new TransferUtility(_amazonS3);
                    await fileTransferUtility.UploadAsync(uploadRequest);

                    await _metaFiles.InsertOneAsync(res);
                    return res;

                }
                return null;

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

        public static async Task DeleteBucketAsync(IAmazonS3 client, string bucketName)
        {
            ListObjectsRequest listRequest = new ListObjectsRequest
            {
                BucketName = bucketName
            };

            ListObjectsResponse listResponse;
            do
            {
                // Get a list of objects
                listResponse = await client.ListObjectsAsync(listRequest);
                foreach (S3Object obj in listResponse.S3Objects)
                {
                    // Delete each object
                    await client.DeleteObjectAsync(new DeleteObjectRequest
                    {
                        BucketName = obj.BucketName,
                        Key = obj.Key
                    });
                }

                // Set the marker property
                listRequest.Marker = listResponse.NextMarker;
            } while (listResponse.IsTruncated);

            // Construct DeleteBucket request
            DeleteBucketRequest request = new DeleteBucketRequest
            {
                BucketName = bucketName
            };

            // Issue call
            DeleteBucketResponse response = await client.DeleteBucketAsync(request);
        }
    }
}
