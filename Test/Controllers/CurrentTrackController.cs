using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schober.Felix.ITunes.Controller;
using Schober.Felix.ITunes.Controller.Model;

namespace Test.Controllers
{
    [Produces("application/json")]
    [Route("api/CurrentTrack")]
    public class CurrentTrackController : Controller
    {
        [HttpGet]
        public Track Get()
        {
            var currentTrack = ITunesService.Instance.GetCurrenTrack();
            return currentTrack;
        }

        public HttpResponseMessage GetCover()
        {
            var track = ITunesService.Instance.GetCurrenTrack();
            var coverPath = track.GetPathToTrackArtwork();
            var coverFormat = track.GetArtworkFileExtension();

            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StreamContent(new FileStream(coverPath, FileMode.Open)); // this file stream will be closed by lower layers of web api for you once the response is completed.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            return response;


        }
    }
}