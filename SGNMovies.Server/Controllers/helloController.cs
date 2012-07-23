using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SGNMovies.Server.Models;


namespace SGNMovies.Server.Controllers
{
    public class helloController : Controller
    {
        //
        // GET: /hello/

        public ActionResult Index()
        {

            SGNMovieContainer isgnmovies = new SGNMovieContainer();
            //MovieController movie = new MovieController(isgnmovies);

            CinemaController session = new CinemaController(isgnmovies);

            return session.List();
        }

    }
}
