using Core.Entites;
using Core.IRepositories;
using Core.IServices;
using Core.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ReminderService : BackgroundService
    {

        private readonly IServiceProvider serviceProvider;

        public ReminderService(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken StoppingToken)
        {
            while (!StoppingToken.IsCancellationRequested)
            {
                using var Scope = serviceProvider.CreateScope();
                var UnitOfWork = Scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var EmailSetting = Scope.ServiceProvider.GetRequiredService<IEmailSetting>();

                var Tomorrow = DateTime.Today.AddDays(1);
                var Spec = new BookingSpec(Tomorrow);
                var BookedEvents = await UnitOfWork.Entity<Booking>().GetAllIncldedWithSpec(Spec);
                foreach(var BookedEvent in BookedEvents)
                {
                    var email = new Email()
                    {
                        To = BookedEvent.AppUser.Email,
                        Subject = "Reminder",
                        Body = $"Hello {BookedEvent.AppUser.UserName},\n\nThis is a reminder that your event \"{BookedEvent.Event.Name}\" will take place tomorrow at {BookedEvent.Event.Date:hh:mm tt} in {BookedEvent.Event.Venue}.\n\nBest regards,\nEvent Booking Team"
                    };
                    EmailSetting.SendEmail(email);
                }
                await Task.Delay(TimeSpan.FromDays(1), StoppingToken);
            }
        }
    }
}
