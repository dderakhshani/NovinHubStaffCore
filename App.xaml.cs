using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NovinDevHubStaffCore.Services;
using Refit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace NovinDevHubStaffCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            Services = ConfigureServices();
            Ioc.Default.ConfigureServices(Services);
        }

        public IServiceProvider Services { get; }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

          
            var retfitSetting= new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        })
            };

           
            services.AddSingleton<ISettingsService, SettingsService>();
            //-----------------Auth Service-----------------------
            services.AddSingleton(RestService.For<IAuthService>("https://kc.novin.dev/", retfitSetting));
            var authTokenStore = new AuthTokenStore();
            services.AddSingleton<IAuthTokenStore, AuthTokenStore>();


            //-----------------Task/Project Service-----------------------
            var httpProjectClient = new HttpClient(new AuthHeaderHandler(authTokenStore))
            {
                BaseAddress = new Uri("https://lb.novin-dev.com/task"), 
                 
            };
            httpProjectClient.DefaultRequestHeaders.Add("apikey", "ZH4KP8HxzhupxNwUVrpk3drSERWZe9RJ");
            httpProjectClient.DefaultRequestHeaders.Add("realm", "nhc");

            services.AddSingleton(RestService.For<IProjectService>(httpProjectClient, retfitSetting));
            services.AddSingleton(RestService.For<ITaskService>(httpProjectClient, retfitSetting));

            //-----------------Sync Service-----------------------
            var httpSyncClient = new HttpClient(new HttpLoggingHandler(authTokenStore))
            {
                BaseAddress = new Uri("https://lb.novin-dev.com/hubstaff"),
            };
            httpSyncClient.DefaultRequestHeaders.Add("realm", "nhc");
            services.AddSingleton(RestService.For<IApplicationService>(httpSyncClient, retfitSetting));
            services.AddSingleton(RestService.For<IActivityService>(httpSyncClient, retfitSetting));
            services.AddSingleton(RestService.For<IUrlService>(httpSyncClient, retfitSetting));
            services.AddSingleton(RestService.For<IScreenshotService>(httpSyncClient, retfitSetting));

            var httpFileClient = new HttpClient(new HttpLoggingHandler(authTokenStore))
            {
                BaseAddress = new Uri("https://lb.novin-dev.com"),
            };
            services.AddSingleton(RestService.For<IFileService>(httpFileClient, retfitSetting));

            return services.BuildServiceProvider();
        }
    }
}
