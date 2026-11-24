using ClassLib.Interface;
using ClassLib.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLib.Repo
{
    public class ProjectRepo : IRepo<Project>
    {

        private BorumBygDBContext _context;
        public ProjectRepo()
        {
            _context = new BorumBygDBContext();
        }

        public Project Add(Project entity)
        {
            _context.Projects.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public Project Delete(int id)
        {
            var project = _context.Projects.Find(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
            return project;
        }

        public List<Project> GetAll()
        {
            return _context.Projects.ToList();
        }

        public Project GetById(int id)
        {
            return _context.Projects.Find(id);
        }

        public Project Update(Project entity, int id)
        {
            var project = GetById(id);
            if (project != null)
            {
                _context.Entry(project).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }
            return project;
        }

        /// <summary>
        /// Gets the projects associated with a customer.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <returns>A list of projects associated with the specified customer.</returns>
        public List<Project> GetProjectsByCustomerId(int customerId)
        {
            return _context.Projects.Where(p => p.CustomerId == customerId).ToList();
        }

    }
}
