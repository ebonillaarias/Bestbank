using AutoMapper;
using inConcert.iMS.DataAccess.Data;
using inConcert.iMS.ServiceAgent.FirebaseCM;
using inConcert.iMS.ServiceAgent.Siebel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace inConcert.iMS.Api
{
   public class Startup
   {
      public IConfiguration Configuration { get; }
      public bool IsDevelopment { get; }
      
      public Startup(IConfiguration configuration, IWebHostEnvironment env)
      {
         Configuration = configuration;
         IsDevelopment = env.IsDevelopment();
         //loggerFactory.AddFile("Logs/myapp-{Date}.txt");
      }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices(IServiceCollection services)
      {
            //InConcertDbContext
            var encryptedDefaultConnectionData = Configuration.GetConnectionString("DefaultConnection");
            var decryptedDefaultConnectionData = Utils.Decrypt(encryptedDefaultConnectionData);

            services.AddDbContext<InConcertDbContext>(options => options.UseSqlServer(decryptedDefaultConnectionData, assembly => assembly.MigrationsAssembly(typeof(InConcertDbContext).Assembly.FullName)));

            //Prueba
            //services.AddDbContext<InConcertDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers(x =>
         {
            var authenticatedUserPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();

            x.Filters.Add(new AuthorizeFilter(authenticatedUserPolicy));
         }).AddNewtonsoftJson();

         services.AddHttpClient();
         services.AddHttpClient<ISiebelClient, SiebelClient>(client =>
         {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", Configuration.GetValue<string>("ExternalServicesHosts:Siebel:UserAgent"));
            client.DefaultRequestHeaders.Add("Authorization", Configuration.GetValue<string>("ExternalServicesHosts:Siebel:HeaderTokenAuthorization"));
            client.DefaultRequestHeaders.Add("Host", Configuration.GetValue<string>("ExternalServicesHosts:Siebel:Host"));
            client.BaseAddress = new Uri(Configuration.GetValue<string>("ExternalServicesHosts:Siebel:BaseAddress"));
         });

         services.AddHttpClient<IFirebaseClient, FirebaseClient>(client =>
         {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", Configuration.GetValue<string>("ExternalServicesHosts:FCM:HeaderTokenAuthorization"));
            client.DefaultRequestHeaders.Add("Host", Configuration.GetValue<string>("ExternalServicesHosts:FCM:Host"));
            client.BaseAddress = new Uri(Configuration.GetValue<string>("ExternalServicesHosts:FCM:BaseAddress"));
         });

         services.AddControllers().AddNewtonsoftJson();

         services.AddCors(options =>
         {
            options.AddPolicy(
                   "CorsPolicy",
                   builder =>
                       builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           );
         });

         //if (IsDevelopment)
         {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
               c.SwaggerDoc(SwaggerConfiguration.SwaggerConfiguration.DocNameV1, new OpenApiInfo
               {
                  Title = SwaggerConfiguration.SwaggerConfiguration.DocInfoTitle,
                  Version = SwaggerConfiguration.SwaggerConfiguration.DocInfoVersion,
                  Description = SwaggerConfiguration.SwaggerConfiguration.DocInfoDescription,
                  Contact = new OpenApiContact
                  {
                     Name = SwaggerConfiguration.SwaggerConfiguration.ContactName,
                     Email = string.Empty,
                     //Url = new Uri(SwaggerConfiguration.SwaggerConfiguration.ContactUrl),
                  }
               });

               // Set the comments path for the Swagger JSON and UI.
               var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
               var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
               c.IncludeXmlComments(xmlPath);
            });
         }


         services.AddAuthentication(options =>
         {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
         }).AddJwtBearer(x =>
         {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration.GetSection("Auth:Key").Value)),
               ValidateIssuer = false,
               ValidateAudience = false,
            };
         });

         // Auto Mapper
         services.AddAutoMapper(typeof(Startup));

         services.AddApplicationServices(Configuration);

         services.Configure<IISServerOptions>(options =>
         {
            options.AutomaticAuthentication = false;
         });
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app)
      {
         if (IsDevelopment)
         {
            app.UseDeveloperExceptionPage();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
               c.SwaggerEndpoint(SwaggerConfiguration.SwaggerConfiguration.EndPointUrl, SwaggerConfiguration.SwaggerConfiguration.EndPointDescription);
               c.RoutePrefix = string.Empty;
            });
         }

         //app.UseHttpsRedirection();

         app.UseRouting();

         app.UseCors("CorsPolicy");

         // Añadimos el middleware de autenticacion de usuarios al pipeline  de ASP.NET Core
         app.UseAuthentication();

         app.UseAuthorization();

         app.UseEndpoints(endpoints =>
         {
            endpoints.MapControllers();
         });
      }
   }
}
