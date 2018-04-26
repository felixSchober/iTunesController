using System;
using System.IO;
using iTunesLib;

namespace Schober.Felix.ITunes.Controller.Model
{
    public class Track
    {
        public string Album { get; }
        public string Artist { get; }
        public string Name { get; }
        private readonly IITArtwork _artwork;
        private IITTrack _track;

        internal Track(IITTrack track)
        {
            Album = track.Album;
            Artist = track.Artist;
            Name = track.Name;
            _track = track;

            if (track.Artwork.Count > 0)
            {
                // iTunes starts at 1(!)
                _artwork = track.Artwork[1];
            }
        }

        public string GetPathToTrackArtwork()
        {
            if (_artwork == null) return "";

            // itunes doesn't provide an easy way to "get" the artwork. We have to save it and then load it again to get it
            var filepath = $"{Path.GetTempPath()}{Name}_{Artist}.{GetArtworkFileExtension()}";
            Console.WriteLine("Saved Temp Artwork to " + filepath);
            try
            {
                _artwork.SaveArtworkToFile(filepath);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("File probably already exists");
                Console.WriteLine(e);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return filepath;

        }

        public string GetArtworkFileExtension()
        {
            if (_artwork == null) return "";

            var format = _artwork.Format;
            switch (format)
            {
                case ITArtworkFormat.ITArtworkFormatBMP:
                    return "bmp";
                case ITArtworkFormat.ITArtworkFormatJPEG:
                    return "jpeg";
                case ITArtworkFormat.ITArtworkFormatPNG:
                    return "png";
                default:
                    return "bin";
            }
        }
    }
}