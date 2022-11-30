using Amazon.S3.Model;
using JustChat.DAL.Entities;
using JustChat.DAL.ViewModel;
using Microsoft.AspNetCore.Http;

namespace JustChat.BLL.Interfaces
{
    public interface IFileService
    {
        Task CreateBucketAsync( string name );

        Task<List<S3Bucket>> GetAllBucketsAsync();

        Task<MetaFile> PostFileAsync( MessageRequest mess );

        Task<Stream> GetFileAsync( string bucketName, string fileKey );

        Task<List<MetaFile>> GetAllFilesAsync();
    }
}
