using CoverFetcherService;
using Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<CoverFetcherWorker>();
builder.Services.AddHttpClient<MusicBrainzFetcher>(client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("UniversityProjectScrobbler/0.0.1 ( glock040@proton.me )");
});

builder.Services.AddSingletonInfrastructure(builder.Configuration);


var host = builder.Build();
host.Run();