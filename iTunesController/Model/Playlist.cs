using System;
using System.Collections.Generic;
using System.Text;
using iTunesLib;

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

            //playlist.GetITObjectIDs(out int sourceId, out int playlistId, out int trackId,
            //    out int databaseId);
            //Id = playlistId;
        }

        public Track Play()
        {
            _playlist.PlayFirstTrack();
            return Songs > 0 ? TrackList[0] : null;
        }
    }
}
