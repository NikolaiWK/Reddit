using Microsoft.EntityFrameworkCore;
using RedditAPI.Data;
using RedditAPI.DataObjects;
using RedditDomain.Entities;
using Thread = RedditDomain.Entities.Thread;

namespace RedditAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<RedditContext>(options => options.UseSqlite($"Data Source=bin/RedditDatabase.db"));
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("myAppCors", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            
            /*Thread endpoints*/
            var threads = app.MapGroup("/api/thread");

            threads.MapGet("/", async (RedditContext context) =>
            {
                var listDto = await context.Threads.Include(t=>t.Comments).Select(t=>MappingProfile.MapThreadToDto(t)).ToListAsync();

                return Results.Ok(listDto);
            });
            threads.MapGet("/{id}",
                async (int id, RedditContext context) =>
                {
                    var existingThread = await context.Threads.Include(t=>t.Comments).FirstOrDefaultAsync(t=>t.Id == id);
                    if (existingThread != null)
                    {
                        var threadDto = MappingProfile.MapThreadToDto(existingThread);
                        return Results.Ok(threadDto);
                    }

                    return Results.NotFound();

                });
            threads.MapPost("/", async (CreateThreadDto threadDto, RedditContext context) =>
            {
                var thread = new Thread(threadDto.AuthorName, threadDto.Title, threadDto.Content);
                await context.Threads.AddAsync(thread);

                await context.SaveChangesAsync();

                return Results.Created($"/api/thread/{thread.Id}", MappingProfile.MapThreadToDto(thread));
            });
            threads.MapPost("/{id}/upvote", async (int id, RedditContext context) =>
            {
                var thread = await context.Threads.FindAsync(id);
                if (thread == null) return Results.NotFound();
                thread.UpvoteCount++;
                await context.SaveChangesAsync();
                return Results.Ok();

            });
            threads.MapPost("/{id}/downvote", async (int id, RedditContext context) =>
            {
                var thread = await context.Threads.FindAsync(id);
                if (thread == null) return Results.NotFound();
                thread.DownvoteCount++;
                await context.SaveChangesAsync();
                return Results.Ok();

            });

            /*Comment endpoints*/
            threads.MapPost("/{id}/comments", async (int id, CreateCommentDto commentDto, RedditContext context) =>
            {
                var thread = await context.Threads.Include(t => t.Comments).FirstOrDefaultAsync(t => t.Id == id);

                if (thread == null) return Results.NotFound();

                var comment = new Comment(id, commentDto.AuthorName, commentDto.Title,
                    commentDto.Content);

                thread.Comments.Add(comment);
                
                await context.SaveChangesAsync();

                return Results.Created($"/api/thread/{thread.Id}", MappingProfile.MapThreadToDto(thread));

            });
            threads.MapPost("/{threadId}/comments/{commentId}/upvote", async (int threadId, int commentId, RedditContext context) =>
            {
                var thread = await context.Threads.FindAsync(threadId);
                if (thread == null) return Results.NotFound($"Thread with {threadId} not found");

                var comment = await context.Comments.FindAsync(commentId);
                if (comment == null) return Results.NotFound($"Comment with {commentId} not found");

                comment.UpvoteCount++;

                await context.SaveChangesAsync();
                return Results.Ok();

            });
            threads.MapPost("/{threadId}/comments/{commentId}/downvote", async (int threadId, int commentId, RedditContext context) =>
            {
                var thread = await context.Threads.FindAsync(threadId);
                if (thread == null) return Results.NotFound($"Thread with {threadId} not found");

                var comment = await context.Comments.FindAsync(commentId);
                if (comment == null) return Results.NotFound($"Comment with {commentId} not found");

                comment.DownvoteCount++;

                await context.SaveChangesAsync();
                return Results.Ok();

            });

            using (var scope = app.Services.CreateScope())
            {
                    //Database seed
                    var context = scope.ServiceProvider.GetRequiredService<RedditContext>();
                    await SeedDatabase.SeedDatabaseWithDummyData(context);
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            app.Run();
        }
    }
    
    public static class MappingProfile
    {
        public static ThreadDto MapThreadToDto(Thread thread)
        {
            return new ThreadDto
            {
                Id = thread.Id,
                AuthorName = thread.AuthorName,
                Title = thread.Title,
                Content = thread.Content,
                CreationDate = thread.CreationDate,
                UpvoteCount = thread.UpvoteCount,
                DownvoteCount = thread.DownvoteCount,
                Comments = thread.Comments.Select(MapCommentToDto).ToList()
            };
        }

        public static CommentDto MapCommentToDto(Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                ThreadId = comment.ThreadId,
                AuthorName = comment.AuthorName,
                Title = comment.Title,
                Content = comment.Content,
                CreationDate = comment.CreationDate,
                UpvoteCount = comment.UpvoteCount,
                DownvoteCount = comment.DownvoteCount
            };
        }
    }
}