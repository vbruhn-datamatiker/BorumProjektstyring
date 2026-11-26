using ClassLib.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;


namespace BorumProjektstyring.Pages
{
    public class TaskModel : PageModel
    {

        private readonly VwTaskWithInfoRepo _viewRepo;

        private readonly TaskRepo _taskRepo;

        public TaskModel(VwTaskWithInfoRepo viewRepo, TaskRepo taskRepo)
        {
            _viewRepo = viewRepo;
            _taskRepo = taskRepo;
        }

        //TODO skal alle properties være binded eller ej? Også ift. validering?
        //TODO hvordan sørger jeg for, når en opgave opdateres, at den skriver de nuværende info for opgaven, som brugeren selv så kan slette/rette i? Ligesom Ulriks "opdater" bruger?

        [BindProperty(SupportsGet = true)] //binder TaskId via URL'en. 
        public int TaskId { get; set; }

        public int DepartmentId { get; set; }

        public int LocationId { get; set; }

        [BindProperty]
        [Required(ErrorMessage ="Indtast en gyldig adresse")]
        public string LocationAddress { get; set; }

        public string PostalCode { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string StartDate { get; set; } = string.Empty;

        public string EndDate { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        public int CreatorUserId { get; set; }

        public string CreatorName { get; set; }

        public IActionResult OnGet()
        {
            //HACK tester hjemmesiden af med den task, der har ID = 1 i vores database. Derefter skal jeg finde ud af, hvordan den selv henter den valgte task inde fra hjemmesiden, når du klikker på task X, Y eller z.

            var task = _viewRepo.GetByTaskId(1);

            TaskId = task.TaskId;
            DepartmentId = task.DepartmentId;
                        
            LocationId = task.LocationId;

            LocationAddress = task.LocationAddress;

            PostalCode = task.PostalCode;
            
            Type = task.TaskType;
            Status = task.TaskStatus;

            StartDate = task.TaskStartDate.ToString("dd-MM-yyyy");

            //Viser kun en slutdato, hvis denne eksisterer - ellers vises "-"
            EndDate = task.TaskEndDate?.ToString("dd-MM-yyyy") ?? "-";

            //TODO lav en metode, der omforder ProjectId til Projektnavn i TaskService-klassen
            ProjectId = task.ProjectId;

            CreatorUserId = task.UserId;

            CreatorName = task.CreatorName;

            return Page();
        }

        public IActionResult OnPostDelete()
        {
            

            //Evt lav Try-catch ift. om opgaveid'en eksisterer eller ej

            _taskRepo.Delete(TaskId);


            //Er det den rette side at redirecte til? Tjek op med Christian. Skal linke til den projekt, der ejede opgaven.
            return RedirectToPage("/ProjectPage");
        }

        public IActionResult OnPostUpdate()
        {

            var taskToUpdate = _taskRepo.GetById(TaskId);

            taskToUpdate.Type = Type;
            taskToUpdate.Status = Status;
            taskToUpdate.Location.Address = LocationAddress;
            taskToUpdate.Location.PostalCode = PostalCode;

            //TODO Husk at opdatere TaskID i Update-metoden, så den linker til den opgave, man åbner inde på hjemmesiden.
            _taskRepo.Update(taskToUpdate, TaskId);

            return Page();
        }


    }
}
