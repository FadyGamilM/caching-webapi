using System.Text.Json;
using StackExchange.Redis;


namespace CachingApi.Services;

public class CacheService : ICacheService
{
   private IDatabase _cacheDb;

   public CacheService()
   {
      var redis = ConnectionMultiplexer.Connect("localhost:6379");
      this._cacheDb = redis.GetDatabase();
   }

   public T GetData<T>(string key)
   {
      var value = this._cacheDb.StringGet(key);
      if(! string.IsNullOrEmpty(value)){
         return JsonSerializer.Deserialize<T>(value);
      }else{
         return default;
      }
   }

   public object RemoveData(string key)
   {
      var exists = this._cacheDb.KeyExists(key);
      if (exists) 
         return this._cacheDb.KeyDelete(key);
      
      return false;
   }

   public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
   {
      var expiryTime = expirationTime.DateTime.Subtract(DateTime.Now);
      var isSet = this._cacheDb.StringSet(key, JsonSerializer.Serialize<T>(value), expiryTime);
      if (isSet)
         return true;
      
      return false;
   }
}