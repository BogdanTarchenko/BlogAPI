using System;
using System.Collections.Generic;

namespace HitsBackend.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; }
    public string? Content { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public DateTime? DeleteDate { get; set; }
    public bool IsDeleted { get; set; }

    public Guid AuthorId { get; set; }
    public User Author { get; set; }

    public Guid PostId { get; set; }
    public Post Post { get; set; }

    public Guid? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}