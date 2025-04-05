namespace Ebay.Backend.Middleware
{
    public class NotFoundMiddleware
    {
        private readonly RequestDelegate _next;

        public NotFoundMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // Check if no endpoint matched and response not written
            if (context.Response.StatusCode == 404 && context.GetEndpoint() == null)
            {
                // Log or handle 404 here
                Console.WriteLine($"404 Not Found: {context.Request.Path}");
            }
        }
    }

}