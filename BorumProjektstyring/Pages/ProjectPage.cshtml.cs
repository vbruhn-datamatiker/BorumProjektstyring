using ClassLib.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ClassLib.Model;


namespace BorumProjektstyring.Pages;

public class ProjectPageModel : PageModel
{
    private readonly ProjectRepo _projectRepo;

    public ProjectPageModel()
    {
        _projectRepo = new ProjectRepo();
    }

    [BindProperty]
    public int ProjectId { get; set; }
    [BindProperty]
    public string ProjectTitle { get; set; }
    [BindProperty]
    public string Type { get; set; }
    [BindProperty]
    public DateTime StartDate { get; set; }
    [BindProperty]
    public string History { get; set; }
    List<Project> Projects { get; set; } = new List<Project>();

    public void OnGet()
    {
        try
        {

            Projects = _projectRepo.GetAll();

        }
        catch (Exception)
        {
            // Log the exception or handle it as needed
            Console.WriteLine("An error occurred while retrieving projects.");
        }
    }

    public IActionResult OnPostCreateProject(string projectTitle, string type, DateTime startDate, string history, int customerId, int userId)
    {
        var newProject = new ClassLib.Model.Project
        {
            ProjectTitle = projectTitle,
            Type = type,
            StartDate = startDate,
            History = history,
            CustomerId = customerId,
            UserId = userId
        };
        _projectRepo.Add(newProject);
        return RedirectToPage("/ProjectPage");
    }
}
