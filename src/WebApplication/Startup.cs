using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Paramore.Brighter;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Brighter.MessagingGateway.RMQ;
using WebApplication.Models;

namespace WebApplication
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication", Version = "v1" });
            });

            var settings = new Settings();
            Configuration.Bind("Settings", settings);

            // using standard rabbitmq, version 3.9.7
            var rmqConnectionOutput = new RmqMessagingGatewayConnection
            {
                AmpqUri = new AmqpUriSpecification(new Uri(settings.AMQP)),
                Exchange = new Exchange(settings.AMQPExchange)
            };
            var producer = new RmqMessageProducer(rmqConnectionOutput);

            var messageStoreOuput = new InMemoryMessageStore();

            services.AddBrighter(b =>
                {
                    b.BrighterMessaging = new BrighterMessaging(messageStoreOuput, producer);
                    // b.CommandProcessorLifetime = ServiceLifetime.Singleton; // not reproduced with singleton
                    b.CommandProcessorLifetime = ServiceLifetime.Scoped; // all 3 exceptions reproduced with scoped
                })
                .MapperRegistryFromAssemblies(typeof(Startup).Assembly)
                .AsyncHandlersFromAssemblies(typeof(Startup).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}