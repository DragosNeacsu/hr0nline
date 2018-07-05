using Microsoft.AspNetCore.Identity;
using Nest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.ElasticSearch
{
	public abstract class ElasticBaseStore<TUser, TRole>
		where TUser : ElasticUser
		where TRole : ElasticRole
	{

		#region ctor

		public ElasticBaseStore(IElasticClient nestClient, IOptions<ElasticOptions> options) : this(nestClient, options.Value)
		{

		}

		public ElasticBaseStore(IElasticClient nestClient, ElasticOptions options)
		{

			if (nestClient == null)
			{
				throw new ArgumentException(nameof(nestClient));
			}

			if (options == null)
			{
				throw new ArgumentException(nameof(options));
			}

			if (string.IsNullOrEmpty(options.UsersIndex))
			{
				throw new ArgumentException(nameof(options.UsersIndex));
			}

			if (string.IsNullOrEmpty(options.RolesIndex))
			{
				throw new ArgumentException(nameof(options.RolesIndex));
			}

			if (string.IsNullOrEmpty(options.UserRolesIndex))
			{
				throw new ArgumentException(nameof(options.UserRolesIndex));
			}

			_nestClient = nestClient;

			Options = options;

			EnsureInitialization();

		}

		#endregion

		#region props
		public ElasticOptions Options { get; set; }

		public IElasticClient _nestClient { get; set; }
		#endregion

		#region Private Helpers
		internal void EnsureInitialization()
		{

			if (!_nestClient.IndexExists(Options.UsersIndex).Exists)
			{

				//var createDescriptor = new CreateIndexDescriptor(Options.UsersIndex)
				//	.Settings(s => s
				//		.NumberOfShards(Options.DefaultShards)
				//		.NumberOfReplicas(Options.DefaultReplicas)
				//	).Mappings(m => m
				//		.Map<TUser>(Options.UsersIndex, mm => mm
				//				.AutoMap()
				//		)
				//);

				var createIndexResult = _nestClient.CreateIndex(Options.UsersIndex);

				if (!createIndexResult.IsValid)
				{
					throw new InvalidOperationException($"Error creating index {Options.UsersIndex}. {createIndexResult.ServerError}");

				}

			}

			if (!_nestClient.IndexExists(Options.RolesIndex).Exists)
			{

				//var createDescriptor = new CreateIndexDescriptor(Options.RolesIndex)
				//	.Settings(s => s
				//		.NumberOfShards(Options.DefaultShards)
				//		.NumberOfReplicas(Options.DefaultReplicas)
				//	).Mappings(m => m
				//		.Map<TUser>(Options.RolesIndex, mm => mm
				//				.AutoMap()
				//		)
				//);

				var createIndexResult = _nestClient.CreateIndex(Options.RolesIndex);

				if (!createIndexResult.IsValid)
				{
					throw new InvalidOperationException($"Error creating index {Options.RolesIndex}. {createIndexResult.ServerError}");

				}

			}

			if (!_nestClient.IndexExists(Options.UserRolesIndex).Exists)
			{

				//var createDescriptor = new CreateIndexDescriptor(Options.UserRolesIndex)
				//	.Settings(s => s
				//		.NumberOfShards(Options.DefaultShards)
				//		.NumberOfReplicas(Options.DefaultReplicas)
				//	).Mappings(m => m
				//		.Map<ElasticUserRole>(Options.UserRolesIndex, mm => mm
				//			.AutoMap()
				//		)
				//);

				var createIndexResult = _nestClient.CreateIndex(Options.UserRolesIndex);

				if (!createIndexResult.IsValid)
				{
					throw new InvalidOperationException($"Error creating index {Options.UserRolesIndex}. {createIndexResult.ServerError}");

				}

			}

		}

		internal string GetMethodName([CallerMemberName] string callerMemberName = null)
		{
			return callerMemberName;
		}
		#endregion

	}
}
