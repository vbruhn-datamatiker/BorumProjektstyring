using ClassLib.Model;
using ClassLib.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BorumProjektstyring.Pages
{
    public class CustomerDetailsPageModel : PageModel
    {
        private readonly CustomerRepo _customerRepo = new CustomerRepo();

        public Customer Customer { get; private set; }

        // id comes from the URL: /CustomerDetailsPage/3
        public IActionResult OnGet(int id)
        {
            Customer = _customerRepo.GetById(id);
            if (Customer == null)
            {
                return NotFound();   // 404 if the customer doesn't exist
            }
            return Page();
        }
    }
}