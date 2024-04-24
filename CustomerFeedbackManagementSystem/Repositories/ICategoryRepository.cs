using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerFeedbackManagementSystem.Repositories
{
    public interface ICategoryRepository
    {
        public List<Tuple<string, int>> GetCategories();
    }
}
