
using KafkaMTWebApp2.Events;
using KafkaMTWebApp2.Handlers;
using MassTransit;
using MassTransit.KafkaIntegration;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Reflection;

namespace KafkaMTWebApp2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(@"KafkaMTWebApp2.xml");
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication2: Send delete event", Version = "v1" });
            });


            services.AddMassTransit(x =>
            {
                //x.UsingInMemory((context, cfg) => cfg.ConfigureEndpoints(context));
                x.UsingRabbitMq(configure: (cxt, cfg) =>
                {

                    cfg.Host("localhost", virtualHost: "/", h => {

                        h.Username("guest");
                        h.Password("guest");

                    });

                    cfg.ConfigureEndpoints(cxt);
                });

                x.AddRider(rider =>
                {
                    rider.AddConsumer<OrgCreatedEventConsumer>();
                    rider.AddProducer<OrgDeletedEvent>(nameof(OrgDeletedEvent));

                    rider.UsingKafka((context, k) =>
                    {
                        k.Host("localhost:9092");

                        k.TopicEndpoint<OrgCreatedEvent>(nameof(OrgCreatedEvent), GetUniqueName(nameof(OrgCreatedEvent)), e =>
                        {
                            e.CheckpointInterval = TimeSpan.FromSeconds(10);
                            e.ConfigureConsumer<OrgCreatedEventConsumer>(context);

                            e.CreateIfMissing(t =>
                            {
                                //t.NumPartitions = 2; //number of partitions
                                //t.ReplicationFactor = 1; //number of replicas
                            });
                        });
                    });
                });
            });

            IServiceCollection serviceCollection = services.AddMassTransitHostedService(true);
        }

        private static string GetUniqueName(string eventName)
        {
            string hostName = Dns.GetHostName();
            string callingAssembly = Assembly.GetCallingAssembly().GetName().Name;
            return $"{hostName}.{callingAssembly}.{eventName}";
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            lifetime.ApplicationStopping.Register(() =>
            {
                Console.WriteLine("ApplicationStopping");
            });
        }
    }
}