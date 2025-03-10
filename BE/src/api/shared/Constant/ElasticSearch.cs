using DotNetEnv;

namespace BE.src.api.shared.Constant
{
    public static class ElasticSearch
    {
        static ElasticSearch()
        {
            Env.Load();
        }
        public static string Port = Environment.GetEnvironmentVariable("ELASTICSEARCH_URL") ?? 
            throw new ApplicationException("ElasticSearch port not found in environment variables.");
        public static string Index = Environment.GetEnvironmentVariable("ELASTICSEARCH_INDEX") ??
            throw new ApplicationException("ElasticSearch index not found in environment variables.");
    }
}