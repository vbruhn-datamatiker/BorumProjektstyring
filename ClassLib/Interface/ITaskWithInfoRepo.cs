using ClassLib.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLib.Interface
{
    public interface ITaskWithInfoRepo
    {

        VwTaskWithInfo? GetByTaskId(int taskId);
    }
}
