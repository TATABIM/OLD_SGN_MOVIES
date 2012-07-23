using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SGNMovies.Server.Models;
using SGNMovies.Server.Providers;
using SGNMovies.Server.Utilities;
using System.IO;

namespace SGNMovies.Server.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISGNMovies _sgnMovies;

        public SessionController(ISGNMovies sgnMovies)
        {
            _sgnMovies = sgnMovies;
        }

        //
        // GET: /Session/

        public ActionResult List()
        {
            return Json(from s in _sgnMovies.SessionTimes
                            select Helper.GetSessionTimeObject(s),JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(string id)
        {
            IContentProvider cp = ContentProviderFactory.Create(id);
            // If use Load SessionTime Old Code
            //_sgnMovies.SaveSessionTimes(id.Equals("megastar")
            //                                ? cp.LoadSessionTimes(_sgnMovies.Cinemas.Where(c => c.WebId > 1000))
            //                                : cp.LoadSessionTimes(_sgnMovies.Cinemas.Where(c => c.WebId < 1000)));

            // If use Load SessionTime New Code
            //_sgnMovies.SaveSessionTimes(cp.LoadSessionTimes(_sgnMovies));

            // Itself updates
            List<SessionTime> sessions = new List<SessionTime>();
            if (id.Equals("galaxy"))
            {
                List<Cinema> cinemas = _sgnMovies.Cinemas.Where(c => c.WebId < 1000).ToList();
                foreach (var cinema in cinemas)
                {
                    string content = string.Format(GalaxyProvider.POST_MOVIE_CONTENT, cinema.Id, (cp as GalaxyProvider).TOKEN_KEY);
                    Stream s = WebConnection.PostUrl(GalaxyProvider.GALAXY_SEARCH_LINK, null, content);
                    string[] vars = (cp as GalaxyProvider).LoadVars(s).Split('~');
                    List<Movie> movies = (cp as GalaxyProvider).LoadMoviesFromVars(_sgnMovies.Movies, vars).ToList();
                    foreach (Movie movie in movies)
                    {
                        content = string.Format(GalaxyProvider.POST_SESSION_CONTENT, cinema.WebId, movie.Var,
                                                (cp as GalaxyProvider).TOKEN_KEY);
                        s = WebConnection.PostUrl(GalaxyProvider.GALAXY_SEARCH_LINK, null, content);
                        List<ShowingDateModel> dates = (cp as GalaxyProvider).LoadDateTimeFromCinemaMovie(s).ToList();
                        foreach (ShowingDateModel dateTime in dates)
                        {
                            sessions.AddRange(dateTime.ShowingTimes.Select(time => new SessionTime
                            {
                                Cinema = cinema, Movie = movie, Date = dateTime.ShowingDate, Time = time.DateTime
                            }));
                        }
                    }
                }
            }
            else
            {
                foreach (var cinema in _sgnMovies.Cinemas.Where(c => c.WebId > 1000))
                {
                    Stream s = WebConnection.GetUrl(string.Format((cp as MegaStarProvider).CINEMA_TO_FILM, cinema.WebId), null);
                    IEnumerable<Movie> tmp = (cp as MegaStarProvider).GetMoviesFromCinema(s);
                    IEnumerable<Movie> movies = (cp as MegaStarProvider).LoadMoviesFromVars(_sgnMovies.Movies, tmp);
                    foreach (var movie in movies)
                    {
                        s = WebConnection.GetUrl(string.Format((cp as MegaStarProvider).CINEME_FILM_TO_SESSIONTIME, cinema.WebId, movie.Var), null);
                        IEnumerable<ShowingDateModel> dates = (cp as MegaStarProvider).LoadDateTimeFromCinemaMovie(s);
                        if (dates != null)
                        {
                            foreach (ShowingDateModel date in dates)
                            {
                                sessions.AddRange(date.ShowingTimes.Select(time => new SessionTime
                                {
                                    Cinema = cinema, Movie = movie, Date = date.ShowingDate, Time = time.DateTime
                                }));
                            }
                        }
                    }
                }
            }
            _sgnMovies.SaveSessionTimes(sessions);

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}
