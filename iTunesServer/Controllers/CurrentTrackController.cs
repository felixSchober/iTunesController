using System;
using System.IO;
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
            FileStream image;
            try
            {
                image = ControllerServices.GetCoverForTrack(track);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not get cover for track");
                Console.WriteLine(e);
                return NotFound(e);
            }

            if (image == null)
            {
                return NotFound(track);
            }

            
            return File(image, "image/" + track.GetArtworkFileExtension());
        }
    }
}