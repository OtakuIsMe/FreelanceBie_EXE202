using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BE.src.api.domains.DTOs.Post
{
    public sealed record PostCreatedEvent(
        Guid userId,
        Guid postId, 
        string Title, 
        DateTime CreatedAt);
}