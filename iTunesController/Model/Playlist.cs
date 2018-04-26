using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using iTunesLib;
using System.Drawing.Imaging;


namespace Schober.Felix.ITunes.Controller.Model
{
    public class Playlist
    {
        private readonly List<Track> _tracklist;
        public List<Track> TrackList
        {
            get
            {
                // only convert the tracks if we actually need them
                if (_tracklist.Count > 0) return _tracklist;

                // do not create the track list if the playlist is a folder. In case of the music folder this would contain the 
                // complete music library and converting all tracks could take a while.
                if (!(_playlist is IITUserPlaylist)) return _tracklist;
                if (((IITUserPlaylist) _playlist).SpecialKind == ITUserPlaylistSpecialKind.ITUserPlaylistSpecialKindFolder)
                    return _tracklist;

                // get the playlist tracks ready
                foreach (IITTrack track in _playlist.Tracks)
                {
                    // only include the first 100 tracks
                    if (_tracklist.Count >= 100) break;

                    _tracklist.Add(new Track(track));
                }

                return _tracklist;
            }
        }

        public int Songs => TrackList.Count;
        public int Duration => _playlist.Duration;
        public string Name => _playlist.Name;
        public int Id => _playlist.playlistID;
        public int SourceId => _playlist.sourceID;

        private readonly IITPlaylist _playlist;

        public Playlist(IITPlaylist playlist)
        {
            _tracklist = new List<Track>(playlist.Tracks.Count);
            _playlist = playlist;
        }

        public Track Play()
        {
            _playlist.PlayFirstTrack();
            return Songs > 0 ? TrackList[0] : null;
        }

        public string GetPlaylistArtwork()
        {
            if (Songs == 0) return "";
            if (Songs < 4) return _tracklist[0].GetPathToTrackArtwork();

            var filepath = $"{Path.GetTempPath()}playlistCollage_{GetHashCode()}.jpeg";

            // test if already cached
            if (File.Exists(filepath))
            {
                return filepath;
            }

            // get the first 4 files
            Image[] covers = new Image[4];
            int h = 0, w = 0;
            for (int i = 0, trackCounter = 0; i < covers.Length; i++)
            {
                // try to get valid covers
                string path = "";
                while (trackCounter < Songs)
                {
                    try
                    {
                        path = TrackList[trackCounter].GetPathToTrackArtwork();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        trackCounter++;
                        continue;
                    }

                    trackCounter++;
                    break;
                }

                // if we could not find a cover break here and deliver the first image only
                if (string.IsNullOrWhiteSpace(path))
                {
                    return _tracklist[0].GetPathToTrackArtwork();
                }

                Image cover = Image.FromFile(path);
                h += cover.Height;
                w += cover.Width;

                covers[i] = cover;
            }

            w /= 2;
            h /= 2;

            Bitmap collage = new Bitmap(w, h);
            Graphics g = Graphics.FromImage(collage);
            g.Clear(Color.White);

            int x = 0, y = 0;
            foreach (var cover in covers)
            {
                g.DrawImage(cover, x, y, w / 2, h / 2);

                // new points for next image
                x += cover.Width;

                if (x >= w)
                {
                    // new row
                    x = 0;
                    y += cover.Height;
                }

                cover.Dispose();
            }
            g.Dispose();

            collage.Save(filepath, ImageFormat.Jpeg);
            return filepath;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + TrackList.GetHashCode();
        }

        public override string ToString()
        {
            return Name + " (" + Songs + ")";
        }
    }
}
