using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Schober.Felix.ITunes.Controller.Model;

namespace Schober.Felix.ITunes.Server
{
    public class ControllerServices
    {
        public static FileStream GetCoverForTrack(Track track)
        {
            string coverPath;
            try
            {
                coverPath = track.GetPathToTrackArtwork();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not get cover path for song " + track.Name);
                Console.WriteLine(e);
                throw new Exception("Could not find cover.", e);
            }


            if (string.IsNullOrWhiteSpace(coverPath))
            {
                throw new Exception("There is no cover for this song");
            }

            return System.IO.File.OpenRead(coverPath);
        }

        public static FileStream GetCoverForPlaylist(Playlist playlist)
        {
            var filepath = playlist.GetPlaylistArtwork();
            if (string.IsNullOrWhiteSpace(filepath))
            {
                throw new Exception("Could not get cover artwork.");
            }
            
            return System.IO.File.OpenRead(filepath);
        }
    }
}
