using ClassLib.Repo;
using ClassLib.Model;
using TaskModel = ClassLib.Model.Task;
using LocationModel = ClassLib.Model.Location;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLib.Service
{
    public class TaskService
    {

        private TaskRepo _taskRepo;

        private readonly VwTaskWithInfoRepo _view;//DENS REPO, ikke klassen selv!

        public TaskService()
        {
            _taskRepo = new TaskRepo();
            _view = new VwTaskWithInfoRepo();
        }

        public TaskRepo TaskRepo { get; set; }

        public VwTaskWithInfoRepo View { get; set; }





       

      

    }
}
