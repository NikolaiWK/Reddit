namespace RedditAPI.DataObjects;

public class ThreadDto
{
    public int Id { get; set; }
    public string AuthorName { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTimeOffset CreationDate { get; set; }
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public List<CommentDto> Comments { get; set; }
}