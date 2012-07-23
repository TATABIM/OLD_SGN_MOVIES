using System.Collections.Generic;
using System.Data.Objects;

namespace SGNMovies.Server.Models
{
    public interface ISGNMovies
    {
        ObjectSet<Cinema> Cinemas { get; }
        ObjectSet<Movie> Movies { get; }
        ObjectSet<SessionTime> SessionTimes { get; }

        int SaveChanges();
        void SaveSessionTimes(IEnumerable<SessionTime> sessions);
        void SaveMovies(IEnumerable<Movie> movies);
       
    }
}
