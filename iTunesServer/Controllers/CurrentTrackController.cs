using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Schober.Felix.ITunes.Controller;
using Schober.Felix.ITunes.Controller.Model;

namespace iTunesServer.Controllers
{
    [Route("api/[controller]")]
    public class CurrentTrackController : Controller
    {
        //[HttpGet]
        //public Track Get()
        //{
        //    var currentTrack = ITunesService.Instance.GetCurrenTrack();
        //    return currentTrack;
        //}

        [HttpGet]
        //[Route("api/[controller]/cover")]
        public IActionResult Get()
        {
            var track = ITunesService.Instance.GetCurrenTrack();
            var coverPath = track.GetPathToTrackArtwork();
            var coverFormat = track.GetArtworkFileExtension();

            var image = System.IO.File.OpenRead(coverPath);
            return File(image, "image/jpeg");




        }
    }
}