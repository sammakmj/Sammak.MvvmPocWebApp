using Sammak.MvvmWebApp.ViewModelLayer;
using System.Web.Mvc;

namespace Sammak.MvvmWebApp.Controllers
{
    public class ProductController : Controller
    {
        #region Private Member Variables
        #endregion


        #region Public Properties
        #endregion

        #region Protected Properties
        #endregion

        #region Constructors
        #endregion

        #region Public Endpoints

        // GET: Product
        public ActionResult Product()
        {
            ProductViewModel vm = new ProductViewModel();
            vm.HandleRequest();
            return View(vm);
        }

        [HttpPost]
        public ActionResult Product(ProductViewModel vm)
        {
            vm.IsValid = ModelState.IsValid;
            if (ModelState.IsValid)
            {
                // Handle action by user
                vm.HandleRequest();

                // Rebind controls
                ModelState.Clear();
            }
            return View(vm);
        }

        #endregion


        #region Protected Methods
        #endregion

        #region Private Methods
        #endregion
    }
}