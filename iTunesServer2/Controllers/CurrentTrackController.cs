using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Schober.Felix.ITunes.Controller;
using Schober.Felix.ITunes.Controller.Model;

namespace iTunesServer.Controllers
{
    public class CurrentTrackController : Controller
    {
       // GET api/currentTrack
       [System.Web.Http.HttpGet]
        public Track Get()
        {
            var currentTrack = ITunesService.Instance.GetCurrenTrack();
            return currentTrack;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/currentTrack/cover")]
        public IActionResult GetCover()
        {
            var track = ITunesService.Instance.GetCurrenTrack();
            var coverPath = track.GetPathToTrackArtwork();
            var coverFormat = track.GetArtworkFileExtension();
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            //var response = Request.CreateResponse(HttpStatusCode.OK);
            FileStream fileStream = new FileStream(coverPath, FileMode.Open);
            Image image = Image.FromStream(fileStream);
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);

            return new ContentResult()
            {
                Content = new ByteArrayContent(memoryStream.ToArray()),

        }

            //response.Content = new ByteArrayContent(memoryStream.ToArray());
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            //return response;
            //return Ok("ss");


            //var contents = File.ReadAllBytes(coverPath);
            //var memoryStream = new MemoryStream(contents);

            //var response = Request.CreateResponse(HttpStatusCode.OK);
            //response.Content = new StreamContent(memoryStream);
            //response.Content.Headers.ContentType = new MediaTypeHeaderValue($"image/{coverFormat}");
            //return Ok(response);


            //var result = new FileStreamResult(new FileStream(coverPath, FileMode.Open), $"image/{coverFormat}");
            //result.FileDownloadName = "cover." + coverFormat;

            //return result;
        }
    }
}
