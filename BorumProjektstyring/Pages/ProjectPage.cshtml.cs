using ClassLib.Model;
using ClassLib.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace BorumProjektstyring.Pages;

public class ProjectPageModel : PageModel
{
    private readonly ProjectRepo _projectRepo;
    private readonly UserRepo _userRepo;


    public ProjectPageModel(ProjectRepo projectRepo, UserRepo userRepo)
    {
        _projectRepo = projectRepo;
        _userRepo = userRepo;
    }

    [BindProperty]
    public int ProjectId { get; set; }

    public List<Project> AllProjects { get; private set; } = new();
    public List<Project> Projects { get; private set; } = new();
    public List<User> Users { get; private set; } = new();
    public Project? SelectedProject { get; private set; }
    public int TotalProjectCount => AllProjects.Count;
    public string TypeFilter { get; private set; } = string.Empty;
    public string AssigneeFilter { get; private set; } = string.Empty;
    public string CustomerFilter { get; private set; } = string.Empty;
    public string SortDirection { get; private set; } = "desc";
    public string NextSortDirection => SortDirection == "asc" ? "desc" : "asc";
    public bool ShowCreateForm { get; private set; }

    public void OnGet(
        string? typeFilter,
        string? assigneeFilter,
        string? customerFilter,
        string? sortDirection,
        int? selectedProjectId,
        bool showCreateForm = false)
    {
        AllProjects = _projectRepo.GetAll();
        Users = _userRepo.GetAll();
        TypeFilter = typeFilter ?? string.Empty;
        AssigneeFilter = assigneeFilter ?? string.Empty;
        CustomerFilter = customerFilter ?? string.Empty;
        SortDirection = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? "asc" : "desc";
        ShowCreateForm = showCreateForm;
        SelectedProject = selectedProjectId.HasValue
            ? AllProjects.FirstOrDefault(project => project.ProjectId == selectedProjectId.Value)
            : null;

        var filteredProjects = AllProjects.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(TypeFilter))
        {
            filteredProjects = filteredProjects.Where(project => project.Type == TypeFilter);
        }

        if (!string.IsNullOrWhiteSpace(AssigneeFilter))
        {
            filteredProjects = filteredProjects.Where(project => project.User?.Name == AssigneeFilter);
        }

        if (!string.IsNullOrWhiteSpace(CustomerFilter))
        {
            filteredProjects = filteredProjects.Where(project => project.Customer?.Name == CustomerFilter);
        }

        /// <summary>
        /// We use the StringComparer since danish langauge contains ÆØÅ, and we need to be able to reead this.
        /// </summary>
    
        var danishNameComparer = StringComparer.Create(CultureInfo.GetCultureInfo("da-DK"), ignoreCase: true);
        Projects = (SortDirection == "asc"
                ? filteredProjects.OrderBy(project => project.ProjectTitle, danishNameComparer)
                : filteredProjects.OrderByDescending(project => project.ProjectTitle, danishNameComparer))
            .ToList();
    }

    public IActionResult OnPostCreateProject(string projectTitle, string type, DateTime startDate, string history, int customerId, int userId)
    {
        var newProject = new Project
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

    public IActionResult OnPostDelete()
    {
        if (ProjectId <= 0 || _projectRepo.GetById(ProjectId) == null)
        {
            return NotFound();
        }

        _projectRepo.Delete(ProjectId);
        return RedirectToPage("/ProjectPage");
    }
}
