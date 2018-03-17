using Autofac;
using Stubbl.Identity.Services.EmailSender;

namespace Stubbl.Identity.Services
{
    public class ServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoggingEmailSender>()
                .As<IEmailSender>()
                .SingleInstance();
        }
    }
}