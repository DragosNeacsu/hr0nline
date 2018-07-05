using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace AspNetCore.Identity.ElasticSearch.Tests
{
	public abstract class BaseStoreTests : IDisposable
	{
		internal string _userIndex = "test_user";
		internal string _roleIndex = "test_role";
		internal string _userRoleIndex = "test_user_role";
		internal readonly IElasticClient _nestClient;
		internal readonly CancellationToken NoCancellationToken = CancellationToken.None;

		public BaseStoreTests()
		{

			var builder = new ConfigurationBuilder()					
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
					.AddEnvironmentVariables();
			var configuration = builder.Build();
			var elasticUri = new Uri(configuration["ElasticSearchUri"]);

			_userIndex += $"_{Guid.NewGuid().ToString()}";
			_roleIndex += $"_{Guid.NewGuid().ToString()}";
			_userRoleIndex += $"_{Guid.NewGuid().ToString()}";
			var connectionPool = new StaticConnectionPool(new[] { elasticUri })
			{
				SniffedOnStartup = false
			};
			var esConnectionConfiguration = new ConnectionSettings(connectionPool);
			esConnectionConfiguration.DisableDirectStreaming(true);
			_nestClient = new ElasticClient(esConnectionConfiguration);
		}

		public void Dispose()
		{
			_nestClient.DeleteIndex(_userIndex);
			_nestClient.DeleteIndex(_roleIndex);
			_nestClient.DeleteIndex(_userRoleIndex);
		}
	}
}
