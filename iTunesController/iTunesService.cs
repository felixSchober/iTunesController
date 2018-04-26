using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using iTunesLib;
using Schober.Felix.ITunes.Controller.Model;
using System.Reflection;



namespace Schober.Felix.ITunes.Controller
{

    /// <summary>
    /// A wrapper for http://www.joshkunz.com/iTunesControl/main.html
    /// </summary>
    public class ITunesService : IDisposable
    {
        private static ITunesService _instance;
        private iTunesApp _app;

        public static ITunesService Instance => _instance ?? (_instance = new ITunesService());
        public bool IsMute { get; private set; }

        public bool IsPlaying
        {
            get
            {
                if (!IsActive) return false;
                return _app.PlayerState == ITPlayerState.ITPlayerStatePlaying;
            }
        }

        public int Volume => !IsActive ? 0 : _app.SoundVolume;


        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get
            {
                if (_app != null) return true;

                // try to get process again
                _app = GetiTunesService();
                return _app != null;
            }
        }


        private ITunesService()
        {
            _app = GetiTunesService();
        }

        private iTunesApp GetiTunesService()
        {
            iTunesApp app;
            // only set app if iTunes is running
            if (Process.GetProcessesByName("iTunes").Any())
            {
                app = new iTunesAppClass();
                IsMute = app.Mute;
            }
            else
            {
                app = null;
            }

            return app;
        }


        public Track GetCurrenTrack()
        {
            if (!IsActive) return null;

            var currentTrack = _app.CurrentTrack;

            // return track if running, otherwise null
            return currentTrack == null ? null : new Track(currentTrack);
        }

        public bool? TogglePlayPause()
        {
            if (!IsActive) return null;

            _app.PlayPause();
            return IsPlaying;
        }

        public void Pause()
        {
            if (!IsActive) return;

            _app.Pause();
        }

        public void Play()
        {
            if (!IsActive) return;

            _app.Play();
        }

        public void ToggleMute()
        {
            if (!IsActive) return;
            _app.Mute = !_app.Mute;
            IsMute = _app.Mute;
        }

        public Track SkipTrack()
        {
            if (!IsActive) return null;

            _app.NextTrack();
            return new Track(_app.CurrentTrack);
        }

        public Track PreviousTrack()
        {
            if (!IsActive) return null;

            _app.PreviousTrack();
            return new Track(_app.CurrentTrack);
        }

        public int? ChangeVolume(int ammount = 5)
        {
            if (!IsActive) return null;

            if (_app.SoundVolume + ammount < 0) _app.SoundVolume = 0;
            else if (_app.SoundVolume + ammount > 100) _app.SoundVolume = 100;
            else _app.SoundVolume += ammount;
            return _app.SoundVolume;
        }

        public PlaylistCollection GetPlaylistCollectionTree()
        {
            var playlistCollection = new PlaylistCollection();
            if (!IsActive) return playlistCollection;


            foreach (IITPlaylist playlist in _app.LibrarySource.Playlists)
            {
                playlistCollection.InsertNode(playlist);
            }

            return playlistCollection;
        }

        public IEnumerable<Playlist> GetPlaylists()
        {
            if (!IsActive) yield return null;


            foreach (IITPlaylist playlist in _app.LibrarySource.Playlists)
            {
                yield return new Playlist(playlist);
            }
        }

        public Playlist GetPlaylistById(int sourceId, int playlistId)
        {
            if (!IsActive) return null;

            IITPlaylist playlist;
            try
            {
                playlist = _app.GetITObjectByID(sourceId, playlistId, 0, 0) as IITPlaylist;
            }
            catch (Exception e)
            {   
                Console.WriteLine($"Could not find playlist with source id {sourceId} and playlist id {playlistId}.");
                Console.WriteLine(e);
                return null;
            }
            if (playlist != null) return new Playlist(playlist);
            return null;
        }




        #region IDisposable

        private void ReleaseUnmanagedResources()
        {
            if (_app != null)
            {
                Marshal.FinalReleaseComObject(_app);
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~ITunesService()
        {
            ReleaseUnmanagedResources();
        }

        #endregion
    }
}