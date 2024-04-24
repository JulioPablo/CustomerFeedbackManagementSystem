using CustomerFeedbackManagementSystem.Data;
using CustomerFeedbackManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerFeedbackManagementSystem.Controllers
{
    public class FeedbackReceiverController : Controller
    {
        private readonly CustomerFeedbackManagementSystemContext _context;
        private readonly IFeedbackReceiverRepository _feedbackReceiverRepository;
        private readonly ILogger<FeedbackReceiverController> _logger;

        public FeedbackReceiverController(ILogger<FeedbackReceiverController> logger, IFeedbackReceiverRepository feedbackReceiverRepositoryepository)
        {
            _feedbackReceiverRepository = feedbackReceiverRepositoryepository;
            _logger = logger;
        }

        public IActionResult GetFeedbackReceivers()
        {
            return Json(_feedbackReceiverRepository.GetFeedbackReceivers().Select(c => new { FeedbackReceiverName = c.Item1, FeedbackReceiverId = c.Item2 }));
        }
    }
}
