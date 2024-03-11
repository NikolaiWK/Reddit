namespace RedditAPI.DataObjects;

public class CreateThreadDto
{
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public string Content { get; set; }
}