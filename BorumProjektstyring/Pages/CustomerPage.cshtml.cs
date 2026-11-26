using ClassLib.Model;
using ClassLib.Repo;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BorumProjektstyring.Pages
{
    public class CustomerPageModel : PageModel
    {
        private readonly CustomerRepo _customerRepo = new CustomerRepo();

        public List<Customer> Customers { get; private set; } = new List<Customer>();

        public void OnGet()
        {
            Customers = _customerRepo.GetAll();
        }
    }
}