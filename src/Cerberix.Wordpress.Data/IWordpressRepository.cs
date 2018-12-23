using System.Collections.Generic;
using System.Threading.Tasks;
using Cerberix.Wordpress.Core;

namespace Cerberix.Wordpress.Data
{
    public interface IWordpressRepository
    {
        /// <summary>
        ///     Get post content by id.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="postId"></param>
        /// <returns></returns>
        Task<WordpressPostContentDto> GetPostContent(
            WordpressPostStatusType status,
            string postId
            );

        /// <summary>
        ///     Get number of posts by status.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<int> GetPostCount(
            WordpressPostStatusType status
            );

        /// <summary>
        ///     Get post ids by status, page index, & page size.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IReadOnlyCollection<string>> GetPostIds(
            WordpressPostStatusType status,
            int pageIndex = 0,
            int pageSize = int.MaxValue
            );

        /// <summary>
        ///     Get number of pages using post by page limit.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<int> GetPostPageCount(
            WordpressPostStatusType status,
            int pageSize
            );       
    }
}
