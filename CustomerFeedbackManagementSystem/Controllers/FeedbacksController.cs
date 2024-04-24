using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CustomerFeedbackManagementSystem.Data;
using CustomerFeedbackManagementSystem.Models;
using CustomerFeedbackManagementSystem.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using CustomerFeedbackManagementSystem.Areas.Identity.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CustomerFeedbackManagementSystem.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly CustomerFeedbackManagementSystemContext _context;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ILogger<FeedbacksController> _logger;
        private readonly UserManager<CustomerFeedbackManagementSystemUser> _userManager;

        public FeedbacksController(ILogger<FeedbacksController> logger, IFeedbackRepository feedbackRepository, UserManager<CustomerFeedbackManagementSystemUser> userManager)
        {
            _feedbackRepository = feedbackRepository;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public record CreateFeedbackReq(int CategoryId, int FeedbackReceiverId, string Description);
        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateFeedbackReq req)
        {
            try
            {
                var feedbackId = await _feedbackRepository.Create(User.FindFirst(ClaimTypes.NameIdentifier).Value, req.CategoryId, req.FeedbackReceiverId, req.Description);
                return Created(nameof(Create), feedbackId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Json(new { error = "Something went wrong, please try again later"});
            }
        }

        public record EditFeedbackReq(int FeedbackId, int CategoryId, int FeedbackReceiverId, string Description);
        //// POST: Feedbacks/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] EditFeedbackReq req)
        {
            try
            {
                var feedbackUserId = await _feedbackRepository.FindFeedbackUserId(req.FeedbackId);
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (feedbackUserId != currentUserId)
                {
                    _logger.LogWarning($"User with id {currentUserId} tried to update feedback with id {req.FeedbackId} belongin to user {feedbackUserId} ");
                    return Json(new { error = "Unable to update a feedback that does not belong to you" });
                }

                await _feedbackRepository.Update(req.FeedbackId, req.CategoryId, req.FeedbackReceiverId, req.Description);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Json(new { error = "Something went wrong, please try again later" });
            }
        }

        // POST: Feedbacks/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {

            try
            {
                var feedbackUserId = await _feedbackRepository.FindFeedbackUserId(id);
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (feedbackUserId != currentUserId)
                {
                    _logger.LogWarning($"User with id {currentUserId} tried to delete feedback with id {id} belongin to user {feedbackUserId} ");
                    return Json(new { error = "Unable to delete a feedback that does not belong to you" });
                }

                await _feedbackRepository.Delete(id);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Json(new { error = "Something went wrong, please try again later" });
            }
        }

        public IActionResult LoadData([FromQuery(Name = "timeframe")] string Timeframe, [FromQuery(Name = "category")] string Category = "all")
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skip number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();


                int? CategoryId = null;
                try
                {
                    CategoryId = Category != "all" ? Convert.ToInt32(Category) : null;
                }
                catch (FormatException e)
                {
                    _logger.LogWarning("CategoryId provided by front end was not a string");
                    _logger.LogError(e.Message, e);
                }

                var pageSize = 0;
                try
                {
                    pageSize = length != null ? Convert.ToInt32(length) : 0;
                }
                catch (FormatException e)
                {
                    _logger.LogWarning("Length provided by front end was not a string");
                    _logger.LogError(e.Message, e);
                }

                var startAsInt = 0;

                try
                {
                    startAsInt = Convert.ToInt32(start);
                }
                catch (FormatException e)
                {
                    _logger.LogWarning("Start provided by front end was not a string");
                    _logger.LogError(e.Message, e);
                }


                (List<FeedbackTableDTO> Data, int RecordsTotal) tableData = _feedbackRepository.GetTableData(startAsInt, pageSize, sortColumn, sortColumnDirection, Timeframe, CategoryId);



                return Json(new { draw = draw, recordsFiltered = tableData.RecordsTotal, recordsTotal = tableData.RecordsTotal, data = tableData.Data});

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return Json(new { error = "Something went wrong, please try again later"});
            }

        }
    }
}
