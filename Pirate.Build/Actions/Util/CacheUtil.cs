using Pirate.Build.Models;
using Pirate.Common.Interfaces;
using Pirate.Parser;

namespace Pirate.Build.Actions.Util;

public static class CacheUtil
{
    public static void AddScopeToCache(Scope scope, string fileName, IObjectSerializer objectSerializer, ILogger logger, string path = "./")  
    {
        logger.Info($"Adding {fileName} to cache");
        if (!File.Exists(path + ".pirate/cache/piratecache.json"))
        {
            objectSerializer.SerializeObject(new CacheFile(), "piratecache");
        }

        var cacheFile = objectSerializer.Deserialize<CacheFile>("piratecache");
        cacheFile.Cache[fileName] = scope;
        objectSerializer.SerializeObject(cacheFile, "piratecache");
    }

    public static Scope GetScopeFromCache(string fileName, IObjectSerializer objectSerializer, ILogger logger, string path = "./")
    {
        logger.Info($"Getting scope from cache for {fileName}");

        var cacheFile = objectSerializer.Deserialize<CacheFile>("piratecache");
        return cacheFile.Cache[fileName];
    }
}
