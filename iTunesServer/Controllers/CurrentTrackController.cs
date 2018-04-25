using Microsoft.AspNetCore.Mvc;
using Schober.Felix.ITunes.Controller;
using Schober.Felix.ITunes.Controller.Model;

namespace Schober.Felix.ITunes.Server.Controllers
{
    [Route("api/[controller]")]
    public class CurrentTrackController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        public Track Get()
        {
            var currentTrack = ITunesService.Instance.GetCurrenTrack();
            return currentTrack;
        }

        [HttpGet]
        [Route("cover")]
        public IActionResult GetCover()
        {
            var track = ITunesService.Instance.GetCurrenTrack();
            var coverPath = track.GetPathToTrackArtwork();
            var coverFormat = track.GetArtworkFileExtension();

            var image = System.IO.File.OpenRead(coverPath);
            return File(image, "image/jpeg");
        }
    }
}