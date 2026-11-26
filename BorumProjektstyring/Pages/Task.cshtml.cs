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
        //TODO REGEX?
        public string LocationAddress { get; set; }

        [BindProperty]
        [Required(ErrorMessage ="Indtast et postnummer")]
        [[RegularExpression(@"^\d{4}$", ErrorMessage = "Postnummer skal bestå af præcis 4 tal")]
        public string PostalCode { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Indtast opgavens type eller beskrivelse")]
        //TODO Regex?
        public string Type { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Vælg status på opgaven")]
        //TODO Lav denne som en dropdown menu inde i cshtml'en = Se Validering_opgave!
        public string Status { get; set; } = string.Empty;

        public string StartDate { get; set; } = string.Empty;

        public string EndDate { get; set; } = string.Empty;

        public int ProjectId { get; set; }

        public int CreatorUserId { get; set; }

        public string CreatorName { get; set; }

        public IActionResult OnGet()
        {
            //HACK tester hjemmesiden af med den task, der har ID = 1 i vores database. Derefter skal jeg finde ud af, hvordan den selv henter den valgte task inde fra hjemmesiden, når du klikker på task X, Y eller z. Bør vel bare være TaskId, som er den udfyldte inputparameter?

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

            if (taskToUpdate == null)
            {
                return NotFound(); //Giver en fejl 404-statuskode, som betyder, at ressourcen ikke kan findes. Vises for brugeren i browseren.
            }

            if (taskToUpdate.Location == null)
            {
                return BadRequest("The selected task does not have a location.");
            }

            taskToUpdate.Type = Type;
            taskToUpdate.Status = Status;

            //TODO hvorfor fejl her? Fordi vi ikke selv skal sætte EndDate?
            taskToUpdate.EndDate = EndDate; 

            taskToUpdate.Location.Address = LocationAddress;
            taskToUpdate.Location.PostalCode = PostalCode;

            _taskRepo.Update(taskToUpdate, TaskId);

            //Ifølge chatten kunne Update-metoden i repoet ændres til:
/*
 * public void Update()
{
_dbContext.SaveChanges();
}
*/

            //TODO Hvad skal returneres? Er det Return Page eller Return RedirectToPage?
            return RedirectToPage("/Task", new { TaskId });
}


}
}
