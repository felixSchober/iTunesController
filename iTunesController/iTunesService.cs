using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using iTunesLib;
using Schober.Felix.ITunes.Controller.Model;

namespace Schober.Felix.ITunes.Controller
{
    public class ITunesService : IDisposable
    {
        private static ITunesService _instance;
        private readonly iTunesApp _app;

        public static ITunesService Instance => _instance ?? (_instance = new ITunesService());
        public bool IsMute { get; private set; }


        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive => _app != null;


        private ITunesService()
        {
            // only set app if iTunes is running
            if (Process.GetProcessesByName("iTunes").Any())
            {
                _app = new iTunesAppClass();
                IsMute = _app.Mute;
            }
        }


        public Track GetCurrenTrack()
        {
            if (!IsActive) return null;

            var currentTrack = _app.CurrentTrack;

            // return track if running, otherwise null
            return currentTrack == null ? null : new Track(currentTrack);
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
