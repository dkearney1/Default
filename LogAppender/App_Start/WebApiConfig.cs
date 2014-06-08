using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Tracing;
using LogAppender.Formatters;

namespace LogAppender
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			config.EnableSystemDiagnosticsTracing();
			SystemDiagnosticsTraceWriter writer = new SystemDiagnosticsTraceWriter()
			{
				MinimumLevel = TraceLevel.Info,
				IsVerbose = false
			}; 

			GlobalConfiguration.Configuration.Formatters.Insert(0, new TextMediaTypeFormatter());

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}
