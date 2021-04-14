using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;

namespace ToolKit.Services
{
    public class S3Service
    {
        private readonly IAmazonS3 _client;
        private readonly string _bucketName;
        private readonly ILogger<S3Service> _logger;

        public S3Service(ILogger<S3Service> logger, string access_key, string secret_key, string bucketName)
        {
            _logger = logger;
            _bucketName = bucketName;
            CredentialProfileOptions options = new CredentialProfileOptions
            {
                AccessKey = "access_key",
                SecretKey = "secret_key"
            };
            CredentialProfile profile = new Amazon.Runtime.CredentialManagement.CredentialProfile("basic_profile", options);
            profile.Region = RegionEndpoint.EUWest3;
            NetSDKCredentialsFile netSDKFile = new NetSDKCredentialsFile();
            netSDKFile.RegisterProfile(profile);
            _client = new AmazonS3Client();
            ListBucketsResponse response = _client.ListBucketsAsync().GetAwaiter().GetResult();
        }
        public async Task DeletingAnObject(string path)
        {
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = path
                };

                await _client.DeleteObjectAsync(request);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    _logger.LogInformation("Please check the provided AWS Credentials.");
                    _logger.LogInformation("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    _logger.LogInformation("An error occurred with the message '{0}' when deleting an object", amazonS3Exception.Message);
                }
            }
        }
        public async Task WritingAnObject(string path, byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(data);
                // simple object put
                PutObjectRequest request = new PutObjectRequest()
                {
                    InputStream = ms,
                    BucketName = _bucketName,
                    Key = path
                };
                PutObjectResponse response = await _client.PutObjectAsync(request);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    _logger.LogInformation("Please check the provided AWS Credentials.");
                    _logger.LogInformation("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    _logger.LogInformation("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message);
                }
            }
        }
    }
}