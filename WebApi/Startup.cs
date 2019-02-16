using Core.Entities;
using Core.Interfaces;
using Core.UseCases;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace WebApi
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Repositories
            services.AddScoped<IRaidRepository, RaidRepository>();
            services.AddScoped<IRaidRoleRepository, RaidRoleRepository>();
            services.AddScoped<IRaidEncounterRepository, RaidEncounterRepository>();
            services.AddScoped<IRaidEncounterSquadRepository, RaidEncounterSquadRepository>();
            services.AddScoped<IRaidBossRepository, RaidBossRepository>();
            services.AddScoped<IRaidTemplateRepository, RaidTemplateRepository>();
            services.AddScoped<IRaidEncounterTemplateRepository, RaidEncounterTemplateRepository>();
            services.AddScoped<IRaidEncounterSquadTemplateRepository, RaidEncounterSquadTemplateRepository>();
            services.AddScoped<IRaidEncounterSquadRoleTemplateRepository, RaidEncounterSquadRoleTemplateRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IGuildRepository, GuildRepository>();
            services.AddScoped<IGuildRankRepository, GuildRankRepository>();
            services.AddScoped<IGuildMemberRepository, GuildMemberRepository>();
            services.AddScoped<IRepository<RaidWing>, RaidWingRepository>();
            services.AddScoped<IGuildWarsApi, GuildWarsApi>();
            
            // UseCases
            services.AddScoped<CreateRaid, CreateRaid>();
            services.AddScoped<RemoveRaid, RemoveRaid>();
            services.AddScoped<AddRaidEncounter, AddRaidEncounter>();
            services.AddScoped<RemoveRaidEncounter, RemoveRaidEncounter>();
            services.AddScoped<CreateRaidTemplate, CreateRaidTemplate>();
            services.AddScoped<RemoveRaidTemplate, RemoveRaidTemplate>();
            services.AddScoped<AddRaidEncounterTemplate, AddRaidEncounterTemplate>();
            services.AddScoped<RemoveRaidEncounterTemplate, RemoveRaidEncounterTemplate>();
            services.AddScoped<AddRaidEncounterSquadRoleTemplate, AddRaidEncounterSquadRoleTemplate>();
            services.AddScoped<RemoveRaidEncounterSquadRoleTemplate, RemoveRaidEncounterSquadRoleTemplate>();
            services.AddScoped<CreateUser, CreateUser>();
            services.AddScoped<RemoveUser, RemoveUser>();
            services.AddScoped<CreateGuild, CreateGuild>();
            services.AddScoped<RemoveGuild, RemoveGuild>();
            
            services.AddScoped<PostgresDatabaseInterface, PostgresDatabaseInterface>();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}