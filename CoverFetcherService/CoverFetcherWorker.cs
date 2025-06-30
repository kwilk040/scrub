using Core.Enums;
using Core.Repositories;

namespace CoverFetcherService;

public class CoverFetcherWorker(
    ILogger<CoverFetcherWorker> logger,
    MusicBrainzFetcher fetcher,
    IAlbumRepository albumRepository)
    : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var album = await albumRepository.GetRecordForCoverFetcherAsync();
            if (album == null)
            {
                logger.LogInformation("No albums without cover arts.");
                await Task.Delay(Interval, stoppingToken);
                continue;
            }

            logger.LogInformation("Fetching cover art for {AlbumTitle}", album.Title);

            var test = await fetcher.FetchAlbumCoverAsync(album.Artists.Select(x => x.Name).ToHashSet(), album.Title);
            logger.LogInformation("URL: {Test}", test);
            if (test is null)
            {
                logger.LogWarning("Couldn't fetch cover art for {AlbumTitle} by {AlbumArtists}", album.Title,
                    string.Join(",", album.Artists.Select(x => x.Name)));
                album.CoverStatus = CoverStatus.Error;
                await albumRepository.Update(album);
                await Task.Delay(Interval, stoppingToken);
                continue;
            }

            album.CoverUrl = test;
            album.CoverStatus = CoverStatus.Fetched;
            await albumRepository.Update(album);
            logger.LogInformation("Fetched cover art for {AlbumTitle} by {AlbumArtists}", album.Title,
                string.Join(",", album.Artists.Select(x => x.Name)));

            await Task.Delay(Interval, stoppingToken);
        }
    }
}