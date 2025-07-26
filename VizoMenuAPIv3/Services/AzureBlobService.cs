using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizoMenuAPIv3.Services
{
    public class AzureBlobService
    {
        private readonly string _baseUrl;

        public AzureBlobService(IConfiguration config)
        {
            _baseUrl = config["BlobBaseUrl"] ?? throw new InvalidOperationException("BlobBaseUrl not configured");
        }

        public Uri GetImageUrl(string container, string relativePath)
        {
            var path = $"{_baseUrl}/{container}/{relativePath}".Replace(" ", "%20");
            return new Uri(path);
        }
    }

}
