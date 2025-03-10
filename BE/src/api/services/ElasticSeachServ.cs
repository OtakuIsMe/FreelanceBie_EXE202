using BE.src.api.domains.DTOs.ElasticSearch;
using BE.src.api.domains.DTOs.User;
using BE.src.api.domains.Model;
using BE.src.api.repositories;
using BE.src.api.shared.Constant;
using Nest;

namespace BE.src.api.services
{
    public interface IElasticSeachServ<T> where T : class
    {
        Task SyncAllDataAsync();
        Task<List<T>> GetAllDocumentsAsync();
        Task<List<T>> SearchAsync(List<string> fields, string query);
        Task<List<T>> SuggestAsync(List<string> fields, string prefix);
        // Task<List<T>> SearchUsersAsync(UserSearchingDTO userSearchingDTO);
    }
	public class ElasticSeachServ<T> : IElasticSeachServ<T> where T : class
	{
        private readonly IUserRepo _userRepo;
        private readonly IPostRepo _postRepo;
        private readonly ElasticClient _client;
        private readonly string _indexName;
        private readonly string _port;
        public ElasticSeachServ(IUserRepo userRepo, IPostRepo postRepo)
        {
            _port = ElasticSearch.Port;
            _indexName = ElasticSearch.Index;
            
            var settings = new ConnectionSettings(new Uri(_port))
                .DefaultIndex(_indexName);

            _client = new ElasticClient(settings);    

            _userRepo = userRepo;
            _postRepo = postRepo;
        }

		public async Task<List<T>> GetAllDocumentsAsync()
		{
			var response = await _client.SearchAsync<T>(s => s
                            .MatchAll()
                            .Size(10000));
            return response.Documents.ToList();                
		}


		public async Task<List<T>> SearchAsync(List<string> fields, string query)
        {
            var response = await _client.SearchAsync<T>(s => s
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f.Fields(fields.ToArray()))
                        .Query(query)
                        .Fuzziness(Fuzziness.Auto)
                    ))
            );

            return response.Documents.ToList();
        }

		// public async Task<List<T>> SearchUsersAsync(UserSearchingDTO userSearchingDTO)
        // {
        //     if (string.IsNullOrWhiteSpace(userSearchingDTO.Query))
        //     {
        //         return new List<T>();
        //     }
        //     var response = await _client.SearchAsync<T>(s => s
        //         .Query(q => q
        //             .Bool(b => b
        //                 .Must(
        //                     q.MultiMatch(m => m
        //                         .Fields(f => f
        //                             .Field("name")
        //                             .Field("username")
        //                             .Field("email")
        //                             .Field("phone")
        //                             .Field("city")
        //                             .Field("education"))
        //                         .Query(userSearchingDTO.Query)
        //                         .Fuzziness(Fuzziness.Auto)
        //                     )
        //                 )
        //             )
        //         )
        //     );

        //     return response.Documents.ToList();
        // }

		public async Task<List<T>> SuggestAsync(List<string> fields, string prefix)
        {
            var response = await _client.SearchAsync<T>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Should(fields.Select(field => 
                            new QueryContainerDescriptor<T>()
                                .Prefix(p => p.Field(field).Value(prefix)))
                        .ToArray())
                    ))
            );

            return response.Documents.ToList();
        }

		public async Task SyncAllDataAsync()
		{
			if(typeof(T) == typeof(User))
            {
                var designers = await _userRepo.GetOnlyCustomers();
                await _client.IndexManyAsync(designers);
            }else if(typeof(T) == typeof(PostJob)){
                var posts = await _postRepo.GetAllPosts();
                await _client.IndexManyAsync(posts);
            }
		}
	}
}