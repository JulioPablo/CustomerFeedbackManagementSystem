using CustomerFeedbackManagementSystem.Areas.Identity.Data;
using System;
using System.Diagnostics;

namespace CustomerFeedbackManagementSystem.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int FeedbackReceiverID { get; set; }
        public string UserId { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public DateTime SubmissionDate { get; set; }

        public virtual CustomerFeedbackManagementSystemUser User { get; set; }
        public virtual Category Category { get; set; }
        public virtual FeedbackReceiver FeedbackReceiver { get; set; }
    }
}
