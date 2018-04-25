using System;
using System.Collections.Generic;
using System.Text;
using iTunesLib;

namespace Schober.Felix.ITunes.Controller.Model
{
    public class Playlist
    {
        public List<Track> TrackList { get; }
        public int Songs => TrackList.Count;
        public int Duration => _playlist.Duration;
        public string Name => _playlist.Name;
        private readonly IITPlaylist _playlist;

        public Playlist(IITPlaylist playlist)
        {
            TrackList = new List<Track>(playlist.Tracks.Count);
            _playlist = playlist;

            // get the playlist tracks
            IITTrackCollection tracksCollection = playlist.Tracks;
            foreach (IITTrack track in tracksCollection)
            {
                TrackList.Add(new Track(track));
            }
        }

        public Track Play()
        {
            _playlist.PlayFirstTrack();
            return Songs > 0 ? TrackList[0] : null;
        }
    }
}
