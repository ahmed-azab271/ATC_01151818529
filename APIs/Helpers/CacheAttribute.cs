using Core.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace APIs.Helpers
{
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int expiredTimeInDays;

        public CacheAttribute(int ExpiredTimeInDays)
        {
            expiredTimeInDays = ExpiredTimeInDays;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var CachedService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();

            var CacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var CachedResponce = await CachedService.GetCachedAsync(CacheKey);

            if (!string.IsNullOrEmpty(CachedResponce))
            {
                var contentResult = new ContentResult()
                {
                    Content = CachedResponce,
                    ContentType = "application/json",
                    StatusCode = 200,
                };
                context.Result = contentResult;
                return;
            }
            var ExecutedEndPoint = await next.Invoke();

            if (ExecutedEndPoint.Result is OkObjectResult result)
                await CachedService.CachingAsync(CacheKey, result.Value, TimeSpan.FromDays(expiredTimeInDays));
        }
        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(request.Path); 

            foreach (var (key, value) in request.Query.OrderBy(X => X.Key))
                keyBuilder.Append($"|{key}-{value}");

            return keyBuilder.ToString();
        }
    }
}
