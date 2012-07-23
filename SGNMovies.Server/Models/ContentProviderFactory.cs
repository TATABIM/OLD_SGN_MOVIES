using System;
using SGNMovies.Server.Providers;

namespace SGNMovies.Server.Models
{
    public static class ContentProviderFactory
    {
        public static IContentProvider Create(string id)
        {
            switch (id)
            {
                case "galaxy":
                    return new GalaxyProvider();
                case "megastar":
                    return new MegaStarProvider();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
