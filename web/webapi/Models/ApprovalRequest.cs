using System;
namespace webapi.Models
{
    public class ApprovalRequest
    {
        public string Id { get; set; }
        public bool Approved { get; set; }
        public string? DenialReason { get; set; }
    }
}