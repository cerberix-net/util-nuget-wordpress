using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cerberix.DataAccess;
using Cerberix.Extension.Core;

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
            const string dbQuery = "SELECT p.post_date,p.post_title,p.post_name,p.post_content FROM wp_posts p WHERE p.post_type='post' AND p.post_status=@status AND p.post_name=@postId LIMIT 1;";

            var dbObject = await _dbConnection.QuerySingle<WordpressPostContentDto>(dbQuery, new { status = status.ToString(), postId });
            return dbObject;
        }

        public virtual async Task<int> GetPostCount(
            WordpressPostStatusType status
            )
        {
            const string dbQuery = "SELECT COUNT(1) post_count FROM wp_posts WHERE post_type='post' AND post_status=@status;";

            var dbObject = await _dbConnection.Execute(dbQuery, new { status = status.ToString() });
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

            var dbQuery = $"SELECT DISTINCT p.post_name FROM wp_posts p WHERE p.post_type='post' AND p.post_status=@status ORDER BY 1 DESC LIMIT {pageSize} OFFSET {pageIndex * pageSize};";
            var dbObject = await _dbConnection.Query<WordpressPostDto>(dbQuery, new { status = status.ToString() });
            return dbObject.Select(r => r.post_name).EnsureArray();
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
