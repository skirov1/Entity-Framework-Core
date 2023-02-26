using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using MusicHub.Data;
using MusicHub.Data.Models;
using MusicHub.Initializer;
using System;
using System.Linq;
using System.Text;

namespace MusicHub
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            MusicHubDbContext context = new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            Console.WriteLine(ExportAlbumsInfo(context, 7));
        }
        
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context
                .Albums
                .Where(a => a.ProducerId.Value == producerId)
                .Include(a => a.Producer)
                .Include(a => a.Songs)
                .ThenInclude(s => s.Writer)
                .ToArray()
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs
                        .Select(s => new
                        {
                            SongName = s.Name,
                            SongPrice = s.Price,
                            SongWriter = s.Writer.Name
                        })
                        .OrderByDescending(s => s.SongName)
                        .ThenBy(s => s.SongWriter)
                        .ToArray(),
                    TotalPrice = a.Songs.Sum(s => s.Price)
                    
                })
                .OrderByDescending(a => a.TotalPrice)
                .ToArray();

            var sb = new StringBuilder();


            foreach (var album in albums)
            {
                var order = 0;
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}"); 
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");

                foreach (var song in album.Songs)
                {
                    order++;
                    sb.AppendLine($"---#{order}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.SongPrice:f2}");
                    sb.AppendLine($"---Writer: {song.SongWriter}");
                }

                sb.AppendLine($"-AlbumPrice: {album.TotalPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            throw new NotImplementedException();
        }
    }
}
