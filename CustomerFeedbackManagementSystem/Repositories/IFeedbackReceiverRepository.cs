using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerFeedbackManagementSystem.Repositories
{
    public interface IFeedbackReceiverRepository
    {
        public List<Tuple<string, int>> GetFeedbackReceivers();

    }
}
