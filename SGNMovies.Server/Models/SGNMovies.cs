using System.Collections.Generic;

namespace SGNMovies.Server.Models
{
    public partial class SGNMovieContainer : ISGNMovies
    {
        public void SaveSessionTimes(IEnumerable<SessionTime> sessionTimes)
        {
            foreach (var session in sessionTimes)
                SessionTimes.AddObject(session);

            SaveChanges();
        }

        public void SaveMovies(IEnumerable<Movie> movies)
        {
            foreach (var movie in movies)
                Movies.AddObject(movie);

            SaveChanges();
        }
    }
}