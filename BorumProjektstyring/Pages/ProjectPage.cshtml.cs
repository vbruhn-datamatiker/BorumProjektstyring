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


    public void OnGet()
    {

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
