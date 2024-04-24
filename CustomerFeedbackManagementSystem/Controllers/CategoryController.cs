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
    public class CategoryController : Controller
    {

        private readonly CustomerFeedbackManagementSystemContext _context;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ILogger<CategoryController> logger, ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }


        public IActionResult GetCategories()
        {
            return Json(_categoryRepository.GetCategories().Select(c => new { CategoryName = c.Item1, CategoryId = c.Item2}));
        }
    }
}
