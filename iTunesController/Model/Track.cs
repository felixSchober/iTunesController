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
        public IITArtwork Artwork { get; }
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
                Artwork = track.Artwork[1];
            }
        }

        public string GetPathToTrackArtwork()
        {
            if (Artwork == null) return "";

            // itunes doesn't provide an easy way to "get" the artwork. We have to save it and then load it again to get it
            var filepath = $"{Path.GetTempPath()}{Name}_{Artist}.{GetArtworkFileExtension()}";
            Console.WriteLine("Saved Temp Artwork to " + filepath);
            try
            {
                Artwork.SaveArtworkToFile(filepath);
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
            if (Artwork == null) return "";

            var format = Artwork.Format;
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