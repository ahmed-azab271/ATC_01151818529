using APIs.Helpers.SignalR;
using System.Runtime.CompilerServices;

namespace APIs.Exctensions
{
    public static class AppExc
    {
        public  static WebApplication AddAppExc (this WebApplication app)
        {
            app.MapHub<NotificationHub>("/notificationHub");

            app.UseStaticFiles();

            return app;
        }
    }
}
 