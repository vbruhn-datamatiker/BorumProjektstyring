using ClassLib.Interface;
using ClassLib.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLib.Repo
{
    public class VwTaskWithInfoRepo : ITaskWithInfoRepo
    {

        private readonly BorumBygDBContext _context;

        public VwTaskWithInfoRepo()
        {
            _context = new BorumBygDBContext();
        }

        public VwTaskWithInfo? GetByTaskId(int taskId)
        {
           return _context.VwTaskWithInfos.FirstOrDefault(t => t.TaskId == taskId);
        }




    }
}
