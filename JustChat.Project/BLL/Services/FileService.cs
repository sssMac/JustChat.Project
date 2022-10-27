using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using JustChat.BLL.Interfaces;
using Microsoft.AspNetCore.Http;

namespace JustChat.BLL.Services
{
    public class FileService : IFileService
    {
        private static IAmazonS3 _amazonS3;
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USWest2;
        public FileService(
            IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
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

        public async Task<MemoryStream> PostFileAsync( IFormFile file )
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
                await using var newMemoryStream = new MemoryStream();
                await file.CopyToAsync(newMemoryStream);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = file.FileName,
                    BucketName = bucketName,
                    CannedACL = S3CannedACL.PublicRead
                };

                var fileTransferUtility = new TransferUtility(_amazonS3);
                await fileTransferUtility.UploadAsync(uploadRequest);

                return newMemoryStream;

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

        public async Task<List<S3Object>> GetAllFilesAsync(string bucketName)
        {
            try
            {
                var response = await _amazonS3.ListObjectsAsync(bucketName);

                return response.S3Objects;

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
