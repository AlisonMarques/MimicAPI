using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MimicAPI.Database;
using MimicAPI.V1.Repositories;
using MimicAPI.V1.Repositories.Contracts;
using MimicAPI.V1.Repositories.Contracts.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using MimicAPI.Helpers;
using MimicAPI.Helpers.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MimicAPI
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            #region AutoMapper - Config
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DTOMapperProfile());
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            #endregion
            
            services.AddDbContext<MimicContext>(opt =>
            {
                opt.UseSqlite("Data Source=Database//Mimic.db");
            });
            
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddScoped<IPalavraRepository, PalavraRepository>();
            
            //controle de versionamento
            services.AddApiVersioning(cfg =>
            {
                // essa config gera o headers indicando quais versões a api suporta
                cfg.ReportApiVersions = true;

                //cfg.ApiVersionReader = new HeaderApiVersionReader("api-version");
                
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1,0);
            });

            // Configurando o serviço de documentação do Swagger com as versões
            services.AddSwaggerGen(cfg =>
            {
                //resolvendo o problema de conflitos com as rotas
                cfg.ResolveConflictingActions(apiDescription => apiDescription.First());
               
                // definindo no swagger quantas versões a API possui
                cfg.SwaggerDoc("v2.0", new OpenApiInfo()
                {
                    Title = "MimicAPI - V2.0",
                    Version = "V2.0"
                });
                
                cfg.SwaggerDoc("v1.1", new OpenApiInfo()
                {
                    Title = "MimicAPI - V1.1",
                    Version = "V1.1"
                });
                
                cfg.SwaggerDoc("v1.0", new OpenApiInfo()
                {
                    Title = "MimicAPI - V1.0",
                    Version = "V1.0"
                });
                
                // Configuração para subir os comentários do controller para o Swagger
                var CaminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;
                var NomeProjeto = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var CaminhoArquivoXMLComentario = Path.Combine(CaminhoProjeto, NomeProjeto);
                cfg.IncludeXmlComments(CaminhoArquivoXMLComentario);

                cfg.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v.ToString()}" == docName);

                });
                services.AddMvc(cfg => cfg.Conventions.Add
                    (new ApiExplorerGroupPerVersionConvention()));
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            
            app.UseMvc();

            // aqui vai criar o arquivo do swagger : /swagger/v1/swagger.json
            app.UseSwagger();
            
            // Ativando middlewares para o uso do Swagger
            app.UseSwaggerUI(cfg =>
            {
                cfg.SwaggerEndpoint("/swagger/v2.0/swagger.json", "MimicAPI - V2.0");
                cfg.SwaggerEndpoint("/swagger/v1.1/swagger.json", "MimicAPI - V1.1");
                cfg.SwaggerEndpoint("/swagger/v1.0/swagger.json", "MimicAPI - V1.0");
                cfg.RoutePrefix = String.Empty;
            });



        }
    }
}