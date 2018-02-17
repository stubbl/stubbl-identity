namespace Stubbl.Identity.Services
{
    using Autofac;
    using Stubbl.Identity.Services.EmailSender;

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
