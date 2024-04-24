using CustomerFeedbackManagementSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerFeedbackManagementSystem.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CustomerFeedbackManagementSystemContext _context;
        public CategoryRepository(CustomerFeedbackManagementSystemContext context)
        {
            this._context = context;
        }

        public List<Tuple<string, int>> GetCategories()
        {
            return _context.Category.Select(c => new Tuple<string, int>(c.Name, c.CategoryId)).ToList();
        }
    }
}
