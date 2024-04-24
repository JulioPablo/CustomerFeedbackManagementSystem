using CustomerFeedbackManagementSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerFeedbackManagementSystem.Repositories
{
    public class FeedbackReceiverRepository : IFeedbackReceiverRepository
    {
        private readonly CustomerFeedbackManagementSystemContext _context;
        public FeedbackReceiverRepository(CustomerFeedbackManagementSystemContext context)
        {
            this._context = context;
        }

        public List<Tuple<string, int>> GetFeedbackReceivers()
        {
            return _context.FeedbackReceiver.Select(c => new Tuple<string, int>(c.Name, c.FeedbackReceiverID)).ToList();
        }
    }
}
