using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Owin;
using Owin;
using System;
using System.Configuration;

//[assembly: OwinStartup(typeof(AzureConn.Startup))] 

namespace AzureConn
{
    public partial class Startup
    {
        private static string applicationInsightsConnectionString = ConfigurationManager.AppSettings["ApplicationInsightsConnectionString"];
        private static Boolean isLocal = bool.Parse(ConfigurationManager.AppSettings["isLocal"]);   //redirect URI for Local 

        private static string clientId = ConfigurationManager.AppSettings["AzureConn:ClientId"];
        private static string tenantId = ConfigurationManager.AppSettings["AzureConn:TenantId"];
        private static string LocalRedirectUri = ConfigurationManager.AppSettings["AzureConn:Local:RedirectUri"];
        private static string AzureRedirectUri = ConfigurationManager.AppSettings["AzureConn:Azure:RedirectUri"];
        private static string RedirectUri = string.Empty;
        private static string PostLogoutRedirectUri = ConfigurationManager.AppSettings["AzureConn:PostLogoutRedirectUri"];
        private static string aadInstance = CheckForSlashAtEnd(ConfigurationManager.AppSettings["AzureConn:AADInstance"]);
        private static string adVersion = ConfigurationManager.AppSettings["AzureConn:ADVersion"];

        string aadAuthority = $"{aadInstance}{tenantId}/{adVersion}";
        
        public void Configuration(IAppBuilder app)
        {
            ConfigureLog4etLogging();

            ConfigureAuth(app);
        }

        public void ConfigureLog4etLogging()
        {
            log4net.Config.XmlConfigurator.Configure();
            TelemetryConfiguration.Active.ConnectionString = applicationInsightsConnectionString;
        }
        
        
        public void ConfigureAuth(IAppBuilder app)
        {
            RedirectUri = isLocal ? LocalRedirectUri: AzureRedirectUri;
            PostLogoutRedirectUri = isLocal ? LocalRedirectUri : AzureRedirectUri;

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = aadAuthority,
                    PostLogoutRedirectUri = PostLogoutRedirectUri,
                    RedirectUri = RedirectUri,

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        SecurityTokenValidated = (context) =>
                        {
                            string name = context.AuthenticationTicket.Identity.FindFirst("preferred_username").Value;
                            context.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Name, name, string.Empty));
                            return System.Threading.Tasks.Task.FromResult(0);
                        },
                        AuthenticationFailed = (context) =>
                        {
                            return System.Threading.Tasks.Task.FromResult(0);
                        }
                    }
                });
            app.UseStageMarker(PipelineStage.Authenticate);
        }

        private static string CheckForSlashAtEnd(string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            if (!value.EndsWith("/", StringComparison.Ordinal))
            {
                return value + "/";
            }

            return value;
        }
        
        
    }

}


