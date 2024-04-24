using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerFeedbackManagementSystem.Models
{
    public class FeedbackTableDTO
    {
        public int FeedbackId { get; set; }
        public int FeedbackReceiverID { get; set; }
        public string FeedbackReceiverName { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public string SubmissionDateString { get; set; }
        public DateTime SubmissionDate { get; set; }

    }
}
