using System.Net;
using Microsoft.AspNetCore.Mvc;
using Schober.Felix.ITunes.Controller;

namespace Schober.Felix.ITunes.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PlaylistController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var playlists = ITunesService.Instance.GetPlaylistCollectionTree();
            return Ok(playlists);
        }

        //[HttpGet]
        //[Route("all/list")]
        //public IActionResult GetAllList()
        //{
        //    if (!ITunesService.Instance.IsActive)
        //    {
        //        return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
        //    }

        //    var playlists = ITunesService.Instance.GetPlaylistCollectionTree();
        //    return Ok(playlists);
        //}

        //[HttpGet]
        //[Route("test/{id}")]
        //public IActionResult GetPlaylist(int id)
        //{
        //    if (!ITunesService.Instance.IsActive)
        //    {
        //        return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
        //    }

        //    var playlist = ITunesService.Instance.GetPlaylistById(id);

        //    if (playlist == null) return NotFound(id);
        //    return Ok(playlist);
        //}
    }
}