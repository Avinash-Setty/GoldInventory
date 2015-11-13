using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using GoldInventory.Model;
using Microsoft.AspNet.Identity.Owin;
using GoldInventory.Models;
using GoldInventory.ParseWrappers;

namespace GoldInventory.Controllers
{
    [Authorize]
    public class MeController : ApiController
    {
        private PUserManager _userStore;

        public MeController()
        {
        }

        public MeController(PUserManager userStore)
        {
            UserStore = userStore;
        }

        public PUserManager UserStore
        {
            get
            {
                return _userStore ?? HttpContext.Current.GetOwinContext().GetUserManager<PUserManager>();
            }
            private set
            {
                _userStore = value;
            }
        }

        // GET api/Me
        public GetViewModel Get()
        {
            //var user = UserStore.FindById(User.Identity.GetUserId());
            return new GetViewModel();
        }

        public async Task<Company> GetCompanyInfo()
        {
            var info = await new CompanyHelper().GetCurrentCompany();
            return info;
        }
    }
}