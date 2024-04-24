using CustomerFeedbackManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerFeedbackManagementSystem.Repositories
{
    public interface IFeedbackRepository
    {
        public Task<int> Create(string UserId, int CategoryId, int FeedbackReceiverId, string Description);
        public (List<FeedbackTableDTO>, int) GetTableData(int Start, int PageSize, string SortColumn, string SortColumnDirection, string Timeframe, int? CategoryId);
        public Task Delete(int FeedbackId);
        public Task<string> FindFeedbackUserId(int FeedbackId);
        public Task Update(int FeedbackId, int CategoryId, int FeedbackReceiverId, string Description);
    }
}
