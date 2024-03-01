using Microsoft.AspNetCore.Http.Extensions;

namespace BlogPost.Middlewar
{
    public class HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await LogRequest(context);

                var original = context.Response.Body;


                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;
                    await next(context);

                    await LogResponse(context, responseBody, original);
                }
            } catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            var requestReader = new StreamReader(context.Request.Body);
            var content = await requestReader.ReadToEndAsync();

            string message = $"Request - url: {context.Request.GetDisplayUrl()}, body: {content}";
            foreach (var (headerKey, headerValue) in context.Response.Headers)
            {
                message += $" header: {headerKey} value: {headerValue}";
            }
            logger.LogInformation(message);
            context.Request.Body.Position = 0;
        }

        private async Task LogResponse(HttpContext context, MemoryStream responseBody, Stream originalResponseBody)
        {
            responseBody.Position = 0;
            var content = await new StreamReader(responseBody).ReadToEndAsync();
            string message = $"Response - body = {content}";

            foreach (var (headerKey, headerValue) in context.Response.Headers)
            {
                message += ($"header?: {headerKey} value: {headerValue}");
            }
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;

            logger.LogInformation(message);
        }
    }
}
