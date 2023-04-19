using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;

namespace ToolKit.Services
{
    public class S3Service : IDisposable
    {
        private readonly IAmazonS3 _client;
        private readonly string _bucketName;
        private readonly ILogger<S3Service> _logger;

        public S3Service(ILogger<S3Service> logger, string bucketName)
        {
            _logger = logger;
            _bucketName = bucketName;
            var options = new CredentialProfileOptions
            {
                AccessKey = "access_key",
                SecretKey = "secret_key",
            };
            var profile = new CredentialProfile("basic_profile", options)
            {
                Region = RegionEndpoint.EUWest3
            };
            var netSDKFile = new NetSDKCredentialsFile();
            netSDKFile.RegisterProfile(profile);
            _client = new AmazonS3Client();
            _ = _client.ListBucketsAsync().GetAwaiter().GetResult();
        }

        public async Task DeletingAnObjectAsync(string path)
        {
            try
            {
                var request = new DeleteObjectRequest()
                {
                    BucketName = _bucketName,
                    Key = path,
                };

                await _client.DeleteObjectAsync(request).ConfigureAwait(true);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId", StringComparison.Ordinal) ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity", StringComparison.Ordinal)))
                {
                    _logger.LogInformation("Please check the provided AWS Credentials.");
                    _logger.LogInformation("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    _logger.LogInformation($"An error occurred with the message '{0}' when deleting an object", amazonS3Exception.Message);
                }
            }
        }

        public async Task WritingAnObjectAsync(string path, byte[] data)
        {
            try
            {
                var ms = new MemoryStream(data);

                // simple object put
                var request = new PutObjectRequest()
                {
                    InputStream = ms,
                    BucketName = _bucketName,
                    Key = path,
                };
                _ = await _client.PutObjectAsync(request).ConfigureAwait(true);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId", StringComparison.Ordinal) ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity", StringComparison.Ordinal)))
                {
                    _logger.LogInformation("Please check the provided AWS Credentials.");
                    _logger.LogInformation("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                }
                else
                {
                    _logger.LogInformation($"An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
        }
    }
}
