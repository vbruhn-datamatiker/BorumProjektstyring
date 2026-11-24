using ClassLib.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using ClassLib.Model;
using TaskModel = ClassLib.Model.Task;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;

namespace ClassLib.Repo
{
    public class TaskRepo : IRepo<TaskModel>
    {

        private BorumBygDBContext _dbContext;

        public TaskRepo()
        {
            _dbContext = new BorumBygDBContext();
        }


        /// <summary>
        /// Adds a task to the database and saves the changes.
        /// </summary>
        /// <param name="task">
        /// The task that are added to the database.
        /// </param>
        /// <returns>
        /// The method returns the added task object.
        /// </returns>
        public TaskModel Add(TaskModel task)
        {

            _dbContext.Tasks.Add(task);

            _dbContext.SaveChanges();

            return task;
            
        }


        /// <summary>
        /// Deletes/removes a task from the database and saves the changes.
        /// </summary>
        /// <param name=id">
        /// The id of the task that are deleted from the database.
        /// </param>
        /// <returns>
        /// The method returns the deleted task object - or throws an exception.
        /// </returns>
        public TaskModel Delete(int id)
        {
            var taskToDelete = _dbContext.Tasks.Find(id);

                     
                _dbContext.Tasks.Remove(taskToDelete);
                _dbContext.SaveChanges();

            if(taskToDelete == null)
            { throw new ArgumentNullException($"Error. A task with id {id} does not exist.");
            }

                return taskToDelete;
            
        }

        /// <summary>
        /// Retrieves all tasks from the database.
        /// </summary>
        /// <returns>
        /// A list containing all tasks in the database. It will return an empty list
        /// if no tasks have been added to the list.
        /// </returns>

        public List<TaskModel> GetAll()
        {
                return _dbContext.Tasks.ToList();
        }




        /// <summary>
        /// Retrieves a task from the database using its id.
        /// </summary>
        /// <param name="id">
        /// The id of the task to retrieve.
        /// </param>
        /// <returns>
        /// The task matching with the specified id. If no task
        /// with that id exists, null is returned.
        /// </returns>
        public TaskModel? GetById(int id)
        {
          
            return _dbContext.Tasks.Find(id);
        }



        /// <summary>
        /// Updates an existing task in the database with updated values and saves the changes to the database.
        /// </summary>
        /// <param name="task">
        /// The task containing the updated values.
        /// </param>
        /// <param name="id">
        /// The id of the task to update.
        /// </param>
        /// <returns>
        /// The task that was found and updated. Returns null if no task with
        /// the specified id exists.
        /// </returns>
        public TaskModel Update(TaskModel task, int id)
        {
            var taskToUpdate = GetById(id);

            if(taskToUpdate == null)
            {
                throw new ArgumentNullException($"Error. A task with id {id} does not exist.");
            }

            if(taskToUpdate != null)
            {
                _dbContext.Entry(taskToUpdate).CurrentValues.SetValues(task);
                _dbContext.SaveChanges();
            }

            
            return taskToUpdate;
        }
    }
}
