using System.Collections.Generic;
using System.IO;
using SGNMovies.Server.Models;
using System.Text;
using System.Linq;
using HtmlAgilityPack;
using System.Web;
using System.Net;

namespace SGNMovies.Server.Providers
{
    public class GalaxyProvider : IContentProvider
    {
        public const string GALAXY_SEARCH_LINK = "http://www.galaxycine.vn/actionServlet";
        public const string POST_MOVIE_CONTENT = "type=reloadmovie&cinema={0}&token={1}";
        public const string POST_SESSION_CONTENT = "type=reloadshowtime&cinema={0}&movie={1}&token={2}";
        public string TOKEN_KEY { get; set; }


        public GalaxyProvider()
        {
            WebResponse response = GetJSessionKey("http://www.galaxycine.vn/");
            TOKEN_KEY = response.Headers.GetValues("Set-Cookie").FirstOrDefault().Split(';').FirstOrDefault();
            TOKEN_KEY = TOKEN_KEY.Replace("JSESSIONID=", "");
        }

        public WebResponse GetJSessionKey(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            return response;
        }

        public int Id
        {
            get { return 1; }
        }

        public string Name
        {
            get { return "Galaxy"; }
        }

        public string BaseUrl
        {
            get { return "http://www.galaxycine.vn"; }
        }

        public IEnumerable<Movie> LoadAllMovies()
        {
            IEnumerable<Movie> now = GetMoviesFromStream(WebConnection.GetUrl(BaseUrl + "/index/vi/movie?type=1", null), true);
            IEnumerable<Movie> coming = GetMoviesFromStream(WebConnection.GetUrl(BaseUrl + "/index/vi/movie?type=2", null), false);

            return coming.Concat(now);
        } 

        public IEnumerable<Movie> LoadNowShowingMovies()
        {
            return GetMoviesFromStream(WebConnection.GetUrl(BaseUrl + "/index/vi/movie?type=1", null), true);
        } 

        public IEnumerable<Movie> LoadComingSoonMovies()
        {
            return GetMoviesFromStream(WebConnection.GetUrl(BaseUrl + "/index/vi/movie?type=2", null), false);
        }

        public IEnumerable<SessionTime> LoadSessionTimes(IEnumerable<Cinema> cinemas)
        {
            foreach (var cinema in cinemas)
            {
                string content = string.Format(POST_MOVIE_CONTENT, cinema.WebId, TOKEN_KEY);
                Stream s = WebConnection.PostUrl(GALAXY_SEARCH_LINK, null, content);
                IEnumerable<Movie> movies = LoadMoviesFromCinema(s);
                foreach (Movie movie in movies)
                {
                    content = string.Format(POST_SESSION_CONTENT, cinema.WebId, movie.Var, TOKEN_KEY);
                    s = WebConnection.PostUrl(GALAXY_SEARCH_LINK, null, content);
                    IEnumerable<ShowingDateModel> dates = LoadDateTimeFromCinemaMovie(s);
                    foreach (ShowingDateModel dateTime in dates)
                    {
                        foreach (var time in dateTime.ShowingTimes)
                        {
                            yield return new SessionTime
                                             {
                                                 Cinema = cinema,
                                                 Movie = movie,
                                                 Date = dateTime.ShowingDate,
                                                 Time = time.DateTime
                                             };
                        }
                    }
                }
            }
        }

        public IEnumerable<SessionTime> LoadSessionTimes(ISGNMovies sgnMovies)
        {
            IEnumerable<Cinema> cinemas = sgnMovies.Cinemas.Where(c => c.WebId < 1000);
            foreach (var cinema in cinemas)
            {
                string content = string.Format(POST_MOVIE_CONTENT, cinema.WebId, TOKEN_KEY);
                Stream s = WebConnection.PostUrl(GALAXY_SEARCH_LINK, null, content);
                string[] vars = LoadVars(s).Split('~');
                IEnumerable<Movie> movies = LoadMoviesFromVars(sgnMovies.Movies, vars);
                foreach (Movie movie in movies)
                {
                    content = string.Format(POST_SESSION_CONTENT, cinema.WebId, movie.Var, TOKEN_KEY);
                    s = WebConnection.PostUrl(GALAXY_SEARCH_LINK, null, content);
                    IEnumerable<ShowingDateModel> dates = LoadDateTimeFromCinemaMovie(s);
                    foreach (ShowingDateModel dateTime in dates)
                    {
                        foreach (var time in dateTime.ShowingTimes)
                        {
                            yield return new SessionTime
                                             {
                                                 Cinema = cinema,
                                                 Movie = movie,
                                                 Date = dateTime.ShowingDate,
                                                 Time = time.DateTime
                                             };
                        }
                    }
                }
            }
        }

        private IEnumerable<Movie> GetMoviesFromStream(Stream stream, bool isNowShowing)
        {
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.Load(reader.BaseStream, true);
            HtmlNodeCollection items = xdoc.DocumentNode.SelectNodes("//div[@class='mov_t_box']");
            return items.Select(item => LoadMovieFromNode(item, isNowShowing)).ToList();
        }

        private Movie LoadMovieFromNode(HtmlNode node, bool isNowShowing)
        {
            return new Movie
            {
                InfoUrl = GetInfoUrl(node),
                ImageUrl = GetImageUrl(node),
                Title = GetTitle(node),
                Genre = GetGenre(node),
                Description = GetDescription(node),
                Var = GetVar(node),// Here is galaxy movie id
                IsNowShowing = isNowShowing,
                Is3d = false,
            };
        }

        #region Get Property Methods

        public string GetTitle(HtmlNode node)
        {
            HtmlNode child = node.SelectSingleNode(node.XPath + "/div[2]/div[1]/div[1]");
            return HttpUtility.HtmlDecode(child.InnerText);
        }

        public string GetGenre(HtmlNode node)
        {
            HtmlNode child = node.SelectSingleNode(node.XPath + "/div[2]/div[1]/div[2]");
            return HttpUtility.HtmlDecode(child.InnerText).Replace("Thể loại", "Thể loại:");
        }

        public string GetDescription(HtmlNode node)
        {
            HtmlNode child = node.SelectSingleNode(node.XPath + "/div[2]/div[1]/div[3]");
            return HttpUtility.HtmlDecode(child.InnerText);
        }

        public string GetInfoUrl(HtmlNode node)
        {
            HtmlNode child = node.SelectSingleNode(node.XPath + "/div[1]/a[1]");
            return child.GetAttributeValue("href", string.Empty);
        }

        public string GetImageUrl(HtmlNode node)
        {
            HtmlNode child = node.SelectSingleNode(node.XPath + "/div[1]/a[1]/img[1]");
            return child.GetAttributeValue("loading", string.Empty);
        }

        public string GetVar(HtmlNode node)
        {
            string detailUrl = GetInfoUrl(node);
            string[] result = detailUrl.Split('/');
            return result[result.Length - 1];
        }

        #endregion Get Property Methods

        #region Load SessionTime Old Code
        private IEnumerable<Movie> LoadMoviesFromCinema(Stream s)
        {
            StreamReader reader = new StreamReader(s, Encoding.UTF8);
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.Load(reader.BaseStream, true);
            HtmlNodeCollection items = xdoc.DocumentNode.ChildNodes;
            return from item in items where item.Name == "div" select LoadMovieFromCinema(item);
        }

        private Movie LoadMovieFromCinema(HtmlNode node)
        {
            HtmlNode span = node.SelectNodes(node.XPath + "/span[@class='cbDisplayMovie']").FirstOrDefault();
            HtmlNode img = node.SelectNodes(node.XPath + "/img[@class='mTooltipMovie']").FirstOrDefault();
            return new Movie
            {
                Var = span.GetAttributeValue("value", string.Empty),
                Title = HttpUtility.HtmlDecode(span.InnerText),
                ImageUrl = img.GetAttributeValue("src", string.Empty),
                InfoUrl = string.Empty,
            };
        }
        #endregion Load SessionTime Old Code

        #region Load SessionTime New Code
        public string LoadVars(Stream s)
        {
            StreamReader reader = new StreamReader(s, Encoding.UTF8);
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.Load(reader.BaseStream, true);
            HtmlNodeCollection items = xdoc.DocumentNode.ChildNodes;
            return items.Aggregate("", (current, item) => current + (LoadVar(item) + "~"));
        }

        private string LoadVar(HtmlNode node)
        {
            if (node.Name.Contains("text"))
                return string.Empty;
            HtmlNode span = node.SelectNodes(node.XPath + "//span[@class='cbDisplayMovie']").FirstOrDefault();
            if (span == null)
                return string.Empty;
            return span.GetAttributeValue("value", string.Empty);
        }

        public IEnumerable<Movie> LoadMoviesFromVars(IEnumerable<Movie> movies, IEnumerable<string> vars)
        {
            List<Movie> results = new List<Movie>();
            foreach (var str in vars)
                if (!string.IsNullOrEmpty(str) && movies != null)
                    results.AddRange(movies.Where(movie => movie.Var == str));

            return results;
        }
        #endregion Load SessionTime New Code

        public IEnumerable<ShowingDateModel> LoadDateTimeFromCinemaMovie(Stream s)
        {
            List<ShowingDateModel> result = new List<ShowingDateModel>();
            StreamReader reader = new StreamReader(s, Encoding.UTF8);
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.Load(reader.BaseStream, true);
            HtmlNodeCollection dates = xdoc.DocumentNode.SelectNodes("//div[@class='showtime_mov_date']");
            HtmlNodeCollection times = xdoc.DocumentNode.SelectNodes("//div[@class='showtime_mov_time']");

            if (dates != null && times != null)
            {
                foreach (HtmlNode date in dates)
                {
                    int index = dates.GetNodeIndex(date);
                    HtmlNode time = times[index];
                    HtmlNodeCollection sessions =
                        time.SelectNodes(time.XPath + "//a[@class='showtime_mov_hour btnShowtime']") ??
                        time.SelectNodes(time.XPath + "//div[@class='showtime_mov_hour readonly']");
                    ShowingDateModel dateTime = new ShowingDateModel
                                                    {ShowingDate = HttpUtility.HtmlDecode(date.InnerText)};

                    foreach (HtmlNode session in sessions)
                    {
                        ShowingTimeModel sessionTime = new ShowingTimeModel
                                                           {
                                                               Id = "",
                                                               DateTime = HttpUtility.HtmlDecode(session.InnerText)
                                                           };
                        dateTime.ShowingTimes.Add(sessionTime);
                    }
                    result.Add(dateTime);
                }
            }
            return result;
        }
    }
}