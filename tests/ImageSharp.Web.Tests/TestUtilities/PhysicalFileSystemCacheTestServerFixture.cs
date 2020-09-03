// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Providers.Azure;
using Xunit;

namespace SixLabors.ImageSharp.Web.Tests.TestUtilities
{
    public class PhysicalFileSystemCacheTestServerFixture : TestServerFixture
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddImageSharp(options =>
            {
                options.OnParseCommandsAsync = context =>
                {
                    Assert.NotNull(context);
                    Assert.NotNull(context.Context);
                    Assert.NotNull(context.Commands);
                    Assert.NotNull(context.Parser);

                    return Task.CompletedTask;
                };

                options.OnProcessedAsync = context =>
                {
                    Assert.NotNull(context);
                    Assert.NotNull(context.Commands);
                    Assert.NotNull(context.ContentType);
                    Assert.NotNull(context.Context);
                    Assert.NotNull(context.Extension);
                    Assert.NotNull(context.Stream);

                    return Task.CompletedTask;
                };

                options.OnBeforeSaveAsync = context =>
                {
                    Assert.NotNull(context);
                    Assert.NotNull(context.Format);
                    Assert.NotNull(context.Image);

                    return Task.CompletedTask;
                };

                options.OnPrepareResponseAsync = context =>
                {
                    Assert.NotNull(context);
                    Assert.NotNull(context.Response);

                    return Task.CompletedTask;
                };
            })
                .ClearProviders()
                .Configure<AzureBlobStorageImageProviderOptions>(options =>
                {
                    options.BlobContainers.Add(new AzureBlobContainerClientOptions
                    {
                        ConnectionString = TestConstants.AzureConnectionString,
                        ContainerName = TestConstants.AzureContainerName
                    });
                })
                .AddProvider(AzureBlobStorageImageProviderFactory.Create)
                .AddProvider<PhysicalFileSystemProvider>()
                .AddProcessor<CacheBusterWebProcessor>();
        }

        protected override void Configure(IApplicationBuilder app)
        {
            app.UseImageSharp();
        }
    }
}
