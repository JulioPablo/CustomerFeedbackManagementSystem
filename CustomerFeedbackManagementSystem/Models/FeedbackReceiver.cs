using System.Collections.Generic;
using System.Diagnostics;

namespace CustomerFeedbackManagementSystem.Models
{
    public class FeedbackReceiver
    {
        public int FeedbackReceiverID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Feedback> ReceivedFeedbacks { get; set; }
    }
}
