using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;

namespace JustChat.API.StartUp
{
    public static class AwsConfiguration
    {
        public static IServiceCollection AwsConnect( this IServiceCollection services, IConfiguration configuration)
        {
            var awsOptions = new AWSOptions
            {
                Credentials = new BasicAWSCredentials(
                    configuration["AWS:AccessKey"],
                    configuration["AWS:AccessSecret"]),

                DefaultClientConfig =
                    {
                        ServiceURL = configuration["AWS:ServiceURL"]
                    },
                Region = Amazon.RegionEndpoint.USWest1
                
            };
            var config = new AmazonS3Config() { ServiceURL = new Uri(configuration["AWS:ServiceURL"]).ToString(), ForcePathStyle = true };
            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonS3>();

            services.AddSingleton<AmazonS3Client>(new AmazonS3Client(configuration["AWS:Secret"], configuration["AWS:Password"], config));

            return services;
        }
    }
}
