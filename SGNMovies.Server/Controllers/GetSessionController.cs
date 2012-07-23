using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SGNMovies.Server.Models;

namespace SGNMovies.Server.Controllers
{
    public class GetSessionController : Controller
    {
        //
        // GET: /GetSession/

        public ActionResult Index()
        {
            SGNMovieContainer isgnmovies = new SGNMovieContainer();
            SessionController session = new SessionController(isgnmovies);

            session.Update("galaxy");

            return session.List();
        }

    }
}
