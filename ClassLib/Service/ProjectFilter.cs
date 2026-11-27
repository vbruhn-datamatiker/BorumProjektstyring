using ClassLib.Interface;
using ClassLib.Model;
using ClassLib.Repo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLib.Service
{
    public class ProjectSearch

    {
        private readonly BorumBygDBContext _context;

        public ProjectSearch (BorumBygDBContext context)
        {
            _context = context;
        }

        public List<Project> SearchByName(string name)
        {
            return _context.Projects
                .Include(project => project.Customer)
                .Where(project => project.ProjectTitle.Contains(name))
                .ToList();
        }
    }
}
