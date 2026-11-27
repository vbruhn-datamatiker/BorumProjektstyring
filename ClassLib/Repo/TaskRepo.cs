using ClassLib.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using ClassLib.Model;
using TaskModel = ClassLib.Model.Task;
using LocationModel = ClassLib.Model.Location;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Models;

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
        public TaskModel? GetById(int taskId)
        {

            return _dbContext.Tasks
                .Include(task => task.Location) //Sørger for, at task'ens location også inkluderes til brug på selve hjemmesiden.
                .FirstOrDefault(task => task.TaskId == taskId);
        }

        //TODO Tjek op, om Update-metoden er skrevet korrekt. Både i dette repo, men også i alle andre repos

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
        public TaskModel Update(TaskModel newTask, int idOnTaskToUpdate)
        {
            var taskToUpdate = GetById(idOnTaskToUpdate);

            if(taskToUpdate == null)
            {
                throw new KeyNotFoundException($"Error. A task with id {idOnTaskToUpdate} does not exist.");
            }

           
                _dbContext.Entry(taskToUpdate).CurrentValues.SetValues(newTask);
                _dbContext.SaveChanges(); //Fortæller EF Core, at ændringerne skal gemmes i vores database.  


            return taskToUpdate;

        }


        //TODO Husk ToString-metoden skal være ens i alle repos


        /// <summary>
        /// ToString method that returns a string representation of our task repo.
        /// </summary>
        /// <returns>A string with information about all the task entities the task repo manages </returns>
        public override string ToString()
        {
            {
                string result = "[ ";
                foreach (TaskModel task in _dbContext.Tasks.ToList())
                {
                    result += task + " ";
                }

                return result + " ]";
            };


          }

       
    }
}
