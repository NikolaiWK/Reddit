using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDomain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int ThreadId { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }


        public Thread Thread { get; set; }

        public Comment(int threadId, string authorName, string title, string content)
        {
            ThreadId = threadId;
            AuthorName = authorName;
            Title = title;
            Content = content;
            CreationDate = DateTimeOffset.Now;
            UpvoteCount = 0;
            DownvoteCount = 0;
        }
    }
}
