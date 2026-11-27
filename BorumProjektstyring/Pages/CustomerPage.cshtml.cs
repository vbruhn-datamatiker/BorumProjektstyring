using ClassLib.Model;
using ClassLib.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BorumProjektstyring.Pages
{
    public class CustomerPageModel : PageModel
    {
        private readonly CustomerRepo _customerRepo = new CustomerRepo();

        public List<Customer> Customers { get; private set; } = new List<Customer>();

        [BindProperty]
        public Customer NewCustomer { get; set; } = new Customer();

        public bool ShowCreateModal { get; private set; }

        public void OnGet()
        {
            Customers = _customerRepo.GetAll();
        }

        // CreateCustomer()
        public IActionResult OnPostCreateCustomer()
        {
            try
            {
                _customerRepo.Add(NewCustomer);
                return RedirectToPage("/CustomerPage"); // Redirect after save means the page won't create the customer twice
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError($"NewCustomer.{ex.ParamName}", ex.Message);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Ny kunde kunne ikke oprettes. Tjek at Ansvarlig-ID er korrekt."); //catch if Ansvarlig-ID doesn't exist
            }

            Customers = _customerRepo.GetAll();
            ShowCreateModal = true;   // reopen the pop-up with the error
            return Page();

        }
    }
}