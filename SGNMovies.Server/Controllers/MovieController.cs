using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SGNMovies.Server.Models;
using SGNMovies.Server.Utilities;

namespace SGNMovies.Server.Controllers
{
    public class MovieController : Controller
    {

        private readonly ISGNMovies _sgnMovies;

        public MovieController(ISGNMovies sgnMovies)
        {
            _sgnMovies = sgnMovies;
        }

        //
        // GET: /Movie/
        public ActionResult List(String type)
        {
            if (type == "nowshowing")
            {
                // Return now showing movies only
                return Json(from m in _sgnMovies.Movies
                            where m.IsNowShowing
                            select Helper.GetMovieObject(m), JsonRequestBehavior.AllowGet);
            }

            if (type == "comingsoon")
            {
                // Return now showing movies only
                return Json(from m in _sgnMovies.Movies
                            where !m.IsNowShowing
                            select Helper.GetMovieObject(m), JsonRequestBehavior.AllowGet);
            }

            // Return all
            return Json(from m in _sgnMovies.Movies
                        select new
                        {
                            Title = m.Title,
                            Director = m.Director,
                            Duration = m.Duration,
                            Genre = m.Genre,
                            Cast = m.Cast,
                            Description = m.Description,
                            Var = m.Var,
                            InfoUrl = m.InfoUrl,
                            ImageUrl = m.ImageUrl,
                            TrailerUrl = m.TrailerUrl,
                            Is3d = m.Is3d,
                            IsNowShowing = m.IsNowShowing,
                            Language = m.Language,
                            Producer = m.Producer,
                            Id = m.Id
                        }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Updating for a provider id
        /// </summary>
        /// <param name="id">Provider id</param>
        /// <returns></returns>
        public ActionResult Update(String id)
        {
            //IContentProvider cp = ContentProviderFactory.Create(id);
            //if (id.Equals("megastar"))
            //    _sgnMovies.SaveSessionTimes(cp.LoadSessionTimes(_sgnMovies.Cinemas.Where(c => c.WebId > 1000)));
            //else
            //{
            //    IEnumerable<Cinema> cinemas = _sgnMovies.Cinemas.Where(c => c.WebId < 1000);
            //    IEnumerable<SessionTime> sessions = cp.LoadSessionTimes(cinemas);
            //    _sgnMovies.SaveSessionTimes(sessions);
            //}

            //return Json("Success", JsonRequestBehavior.AllowGet);

            IContentProvider cp = ContentProviderFactory.Create(id);
                _sgnMovies.SaveMovies(cp.LoadAllMovies());
            
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

    }
}
