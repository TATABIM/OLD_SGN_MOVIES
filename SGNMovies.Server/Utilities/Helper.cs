using System.IO;
using System.Text;
using System.Xml.Linq;
using HtmlAgilityPack;
using SGNMovies.Server.Models;

namespace SGNMovies.Server.Utilities
{
    public static class Helper
    {
        public static string GetAttributeValue(XElement element, string attributeName)
        {
            string result = string.Empty;
            if (element != null)
            {
                var attribute = element.Attribute(attributeName);
                if (attribute != null)
                {
                    result = attribute.Value;
                }
            }
            return result;
        }

        public static string GetElementValue(XElement element, string elementName)
        {
            string result = string.Empty;

            if (element != null)
            {
                var childElement = element.Element(elementName);
                if (childElement != null)
                {
                    return childElement.Value;
                }
            }
            return result;
        }

        public static bool ConvertFromStringToBool(string value)
        {
            return !value.Equals("0");
        }

        public static HtmlDocument ParsingStream(Stream s)
        {
            StreamReader reader = new StreamReader(s, Encoding.UTF8);
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.Load(reader.BaseStream, true);
            return xdoc;
        }

        public static Movie GetMovieObject(Movie m)
        {
            return new Movie
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
                       };
        }

        public static Cinema GetCinemaObject(Cinema c)
        {
            /*return new Cinema
                       {
                           Id = c.Id,
                           Name = c.Name,
                           DisplayName = c.DisplayName,
                           Address = c.Address,
                           Phone = c.Phone,
                           Latitude = c.Latitude,
                           Longitude = c.Longitude,
                           ImageUrl = c.ImageUrl,
                           WebId = 1,

                       };*/

            Cinema cinema = new Cinema();
            cinema.Id = c.Id;
            cinema.Name = c.Name;
            cinema.DisplayName = c.DisplayName;
            cinema.Address = cinema.Address;
            cinema.Phone = c.Phone;
            cinema.Latitude = c.Latitude;
            cinema.Longitude = c.Longitude;
            cinema.ImageUrl = c.ImageUrl;
            cinema.WebId = 1;

            return cinema;

        }

        public static SessionTime GetSessionTimeObject(SessionTime s)
        {
            return new SessionTime
            {
                Id = s.Id,
                Date = s.Date,
                Time = s.Time,
                Cinema = s.Cinema,
                Movie = s.Movie
            };
        }
    }
}