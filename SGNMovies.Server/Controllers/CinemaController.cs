using System.Linq;
using System.Web.Mvc;
using SGNMovies.Server.Models;
using SGNMovies.Server.Utilities;

namespace SGNMovies.Server.Controllers
{
    public class CinemaController : Controller
    {
        private readonly ISGNMovies _sgnMovies;

        public CinemaController(ISGNMovies sgnMovies)
        {
            _sgnMovies = sgnMovies;
        }

        //
        // GET: /Cinema/

        public ActionResult List()
        {


            object data = _sgnMovies.Cinemas;



            //return Json(from c in _sgnMovies.Cinemas
                  //      select Helper.GetCinemaObject(c), JsonRequestBehavior.AllowGet);

            return Json(data, JsonRequestBehavior.AllowGet);
        }



    }
}
