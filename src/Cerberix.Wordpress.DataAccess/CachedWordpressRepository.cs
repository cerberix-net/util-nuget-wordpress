using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cerberix.Caching;
using Cerberix.DataAccess;

namespace Cerberix.Wordpress.DataAccess
{
    public class CachedWordpressRepository : WordpressRepository
    {
        private readonly ICacher _cacher;
        private readonly Func<Type, CacherItemPolicy> _policyFunc;

        public CachedWordpressRepository(
            IDbConnectionAsync dbConnection,
            ICacher cacher,
            Func<Type, CacherItemPolicy> policyFunc
            ) : base(dbConnection)
        {
            _cacher = cacher;
            _policyFunc = policyFunc;
        }

        public override async Task<WordpressPostContentDto> GetPostContent(WordpressPostStatusType status, string postId)
        {
            return await GetFromCacheOrData(
                getCacheItemKey: () => $"{GetType().Name}|{nameof(GetPostContent)}|{postId}",
                getCacheItemFunc: async () => await base.GetPostContent(status, postId)
                );
        }

        public override async Task<int> GetPostCount(WordpressPostStatusType status)
        {
            return await GetFromCacheOrData(
                getCacheItemKey: () => $"{GetType().Name}|{nameof(GetPostCount)}|{status.ToString()}",
                getCacheItemFunc: async () => await base.GetPostCount(status)
                );
        }

        public override async Task<IReadOnlyCollection<string>> GetPostIds(WordpressPostStatusType status, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return await GetFromCacheOrData(
                getCacheItemKey: () => $"{GetType().Name}|{nameof(GetPostIds)}|{pageIndex}|{pageSize}",
                getCacheItemFunc: async () => await base.GetPostIds(status, pageIndex, pageSize)
                );
        }

        public override async Task<int> GetPostPageCount(WordpressPostStatusType status, int pageSize)
        {
            return await GetFromCacheOrData(
                getCacheItemKey: () => $"{GetType().Name}|{nameof(GetPostPageCount)}|{status.ToString()}|{pageSize}",
                getCacheItemFunc: async () => await base.GetPostPageCount(status, pageSize)
                );
        }

        private async Task<TResult> GetFromCacheOrData<TResult>(
            Func<string> getCacheItemKey,
            Func<Task<TResult>> getCacheItemFunc
            )
        {
            var result = await _cacher.GetOrSet(
                cacheItemKey: getCacheItemKey(),
                cacheItemPolicy: _policyFunc(typeof(TResult)),
                getCacheItemFunc: getCacheItemFunc
            );

            return result;
        }
    }
}
