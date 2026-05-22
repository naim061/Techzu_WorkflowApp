using System;
using System.Collections.Generic;
using System.Text;

namespace SponsorshipWorkflow.Domain.Common
{
    public abstract class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
