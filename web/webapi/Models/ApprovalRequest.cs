using System;
namespace webapi.Models
{
    public class ApprovalRequest
    {
        public string Id { get; set; } = null!;
        public bool Approved { get; set; }
        public string? DenialReason { get; set; }
    }
}