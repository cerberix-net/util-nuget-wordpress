using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cerberix.DataAccess.Core;
using Cerberix.Extension.Core;
using Cerberix.Wordpress.Core;

namespace Cerberix.Wordpress.Data
{
    public class WordpressRepository : IWordpressRepository
    {
        private readonly IDbConnectionAsync _dbConnection;

        public WordpressRepository(
            IDbConnectionAsync dbConnection
            )
        {
            _dbConnection = dbConnection;
        }

        public virtual async Task<WordpressPostContentDto> GetPostContent(
            WordpressPostStatusType status,
            string postId
            )
        {
            var dbQuery = $"SELECT p.post_date,p.post_title,p.post_name,p.post_content FROM wp_posts p WHERE p.post_type='post' AND p.post_status='{status.ToString()}' AND p.post_name='{postId}' LIMIT 1;";

            var dbObject = await _dbConnection.QueryScalar<WordpressPostContentDto>(dbQuery);
            return dbObject;
        }

        public virtual async Task<int> GetPostCount(
            WordpressPostStatusType status
            )
        {
            var dbQuery = $"SELECT COUNT(1) post_count FROM wp_posts WHERE post_type='post' AND post_status='{status.ToString()}';";
            var dbObject = await _dbConnection.QueryScalar<int>(dbQuery);
            return dbObject;
        }

        public virtual async Task<IReadOnlyCollection<string>> GetPostIds(
            WordpressPostStatusType status,
            int pageIndex = 0,
            int pageSize = int.MaxValue
            )
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex");
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            var dbQuery = $"SELECT DISTINCT p.post_name FROM wp_posts p WHERE p.post_type='post' AND p.post_status='{status.ToString()}' ORDER BY 1 DESC LIMIT {pageSize} OFFSET {pageIndex * pageSize};";
            var dbObject = await _dbConnection.Query<string>(dbQuery);
            return dbObject.EnsureArray();
        }

        public virtual async Task<int> GetPostPageCount(
            WordpressPostStatusType status,
            int pageSize
            )
        {
            if (pageSize < 0)
            {
                throw new ArgumentOutOfRangeException("pageSize");
            }

            var postCount = await GetPostCount(status);
            if (postCount == 0)
            {
                return 0;
            }

            var pageCount = (postCount / pageSize) + 1;
            return pageCount;
        }

        //public WordpressComment[] GetComments()
        //{
        //    var dbQuery = $"SELECT * FROM wp_comments ORDER BY 1 DESC LIMIT 100;";
        //    return DbConnection.Query<Comment>(dbQuery).ToArray();
        //}

        //public WordpressTerm[] GetTerms()
        //{
        //    var dbQuery = $"SELECT * FROM wp_terms ORDER BY 1 DESC LIMIT 100;";
        //    return DbConnection.Query<Term>(dbQuery).ToArray();
        //}
    }
}
