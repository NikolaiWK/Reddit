using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditDomain.Entities
{
    public class Thread
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }


        public List<Comment> Comments { get; set; }
        
        public Thread(string authorName, string title, string content)
        {
            CreationDate = DateTimeOffset.Now;
            AuthorName = authorName;
            Title = title;
            Content = content;
            UpvoteCount = 0;
            DownvoteCount = 0;
            Comments = new List<Comment>();
        }
    }

}
