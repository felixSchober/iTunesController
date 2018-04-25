using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Schober.Felix.ITunes.Controller;
using Schober.Felix.ITunes.Controller.Model;

namespace Schober.Felix.ITunes.Server.Controllers
{
    [Route("api/[controller]")]
    public class CurrentTrackController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var currentTrack = ITunesService.Instance.GetCurrenTrack();
            return Ok(currentTrack);
        }

        [HttpGet]
        [Route("cover")]
        public IActionResult GetCover()
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var track = ITunesService.Instance.GetCurrenTrack();

            string coverPath;
            try
            {
                coverPath = track.GetPathToTrackArtwork();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not get cover path for song " + track.Name);
                Console.WriteLine(e);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }


            if (string.IsNullOrWhiteSpace(coverPath))
            {
                return NotFound(track);
            }

            var coverFormat = track.GetArtworkFileExtension();

            var image = System.IO.File.OpenRead(coverPath);
            return File(image, "image/" + coverFormat);
        }
    }
}