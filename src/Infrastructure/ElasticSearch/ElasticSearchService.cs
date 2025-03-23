using Domain.Entities;
using Domain.Interfaces;
using Nest;

namespace WebApi.Infrastructure.ElasticSearch
{
    public class ElasticsearchService : IElasticSearchService
    {
        private readonly IElasticClient _elasticClient;

        public ElasticsearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task IndexPermissionAsync(Permission permission)
        {
            var response = await _elasticClient.IndexDocumentAsync(permission);
            if (!response.IsValid)
            {
                // Manejar error si el indexado falla
                throw new Exception("Error indexing permission to Elasticsearch");
            }
        }
    }
}
