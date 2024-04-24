using CustomerFeedbackManagementSystem.Data;
using CustomerFeedbackManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerFeedbackManagementSystem.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly CustomerFeedbackManagementSystemContext _context;
        public FeedbackRepository(CustomerFeedbackManagementSystemContext context)
        {
            this._context = context;
        }

        public async Task<int> Create(string UserId, int CategoryId, int FeedbackReceiverId, string Description)
        {
            var feedback = new Feedback()
            {
                UserId = UserId,
                CategoryId = CategoryId,
                FeedbackReceiverID = FeedbackReceiverId,
                Description = Description,
                SubmissionDate = DateTime.UtcNow
            };

            _context.Feedback.Add(feedback);
            await _context.SaveChangesAsync();
            return feedback.FeedbackId;
        }

        public async Task<string> FindFeedbackUserId(int FeedbackId)
        {
            return (await _context.Feedback.Where(f => f.FeedbackId == FeedbackId).Select(f => f.UserId).ToListAsync()).FirstOrDefault();
        }

        public async Task Delete(int FeedbackId)
        {
            var feedbackToRemove = new Feedback() { FeedbackId = FeedbackId };
            _context.Feedback.Remove(feedbackToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task Update(int FeedbackId, int CategoryId, int FeedbackReceiverId, string Description)
        {
            var feedback = _context.Feedback.Find(FeedbackId);

            feedback.CategoryId = CategoryId;
            feedback.FeedbackReceiverID = FeedbackReceiverId;
            feedback.Description = Description;

            await _context.SaveChangesAsync();
        }


        public (List<FeedbackTableDTO>, int) GetTableData(int Start, int PageSize, string SortColumn, string SortColumnDirection, string Timeframe, int? CategoryId)
        {
            // getting all Customer data  
            var feedbacks = _context.Feedback
                .Include(f => f.Category)
                .Include(f => f.FeedbackReceiver)
                .Include(f => f.User).Select(c => c);

            if (CategoryId is not null)
            {
                feedbacks = feedbacks.Where(f => f.CategoryId == CategoryId);
            }

            switch (Timeframe)
            {
                case "last-day":
                    feedbacks = feedbacks.Where(f => f.SubmissionDate >= DateTime.UtcNow.Date);
                    break;
                case "last-week":
                    var firstDatOftheWeek = (DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek));
                    feedbacks = feedbacks.Where(f => f.SubmissionDate >= firstDatOftheWeek);
                    break;
                case "last-month":
                    var firstDayOfTheMonth = (new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1));
                    feedbacks = feedbacks.Where(f => f.SubmissionDate >= firstDayOfTheMonth);
                    break;
                default:
                    break;
            }

            //total number of rows counts   
            var recordsTotal = feedbacks.Count();

            int skip = Start != null ? Convert.ToInt32(Start) : 0;

            var feedbackDTO = feedbacks.Select(f => new FeedbackTableDTO()
            {
                FeedbackId = f.FeedbackId,
                FeedbackReceiverID = f.FeedbackReceiverID,
                FeedbackReceiverName = f.FeedbackReceiver.Name,
                UserName = f.User.UserName,
                UserId = f.UserId,
                Category = f.Category.Name,
                CategoryId = f.CategoryId,
                Description = f.Description,
                SubmissionDateString = f.SubmissionDate.ToShortDateString(),
                SubmissionDate = f.SubmissionDate
            });

            //Sorting  
            if (!(string.IsNullOrEmpty(SortColumn) && string.IsNullOrEmpty(SortColumnDirection)))
            {
                feedbackDTO = string.Equals(SortColumnDirection, "desc", StringComparison.OrdinalIgnoreCase) ? feedbackDTO.OrderByDescending(f => EF.Property<object>(f, SortColumn)) : feedbackDTO.OrderBy(f => EF.Property<object>(f, SortColumn));
            }

            //Paging   
            var data = feedbackDTO.Skip(skip).Take(PageSize).ToList();

            return (data, recordsTotal);
        }
    }
}
