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
                    configuration["AccessKey"],
                    configuration["SecretKey"]),

                DefaultClientConfig =
            {
                ServiceURL = configuration["ServiceUrl"]
            }
            };

            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonS3>();

            return services;
        }
    }
}
