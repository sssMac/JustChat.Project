using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;

namespace JustChat.BLL.Interfaces
{
    public interface IFileService
    {
        Task CreateBucketAsync( string name );

        Task<List<S3Bucket>> GetAllBucketsAsync();

        Task<MemoryStream> PostFileAsync( string bucketName, IFormFile file );

        Task<Stream> GetFileAsync( string bucketName, string fileKey );

        Task<List<S3Object>> GetAllFilesAsync(string bucketName );
    }
}
