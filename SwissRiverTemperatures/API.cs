using System;

namespace SwissRiverTemperatures
{
    public static class API
    {
        public static String Key = "FrhPnATeSS0dlM1tGooOHOpS1zSGcRFDczGYwuDCWoAy4ZtZ5hP5wa2pXeMHwOZ9ODG6Er5nhyfWm4AFU3E4DW41f3wtcGNMg26QZepSkAFcrEdDbCUQf82ZNTz4wUEs";
        public static String BaseUrl = "http://api.pachube.com/v2/feeds/";
        public static String FeedId = "43397";
        // The following dontCacheMe parameter is used to prevent local caching. See http://stackoverflow.com/q/5173052/284318
        public static String FeedUrl = BaseUrl + FeedId + ".xml?dontCacheMe={0}";
    }
}
