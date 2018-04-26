using System.Net;
using Microsoft.AspNetCore.Mvc;
using Schober.Felix.ITunes.Controller;

namespace Schober.Felix.ITunes.Server.Controllers
{
    [Produces("application/json")]
    [Route("api/Playlist")]
    public class PlaylistController : Microsoft.AspNetCore.Mvc.Controller
    {
        public IActionResult Get()
        {
            return Ok("yes");
        }

        [HttpGet]
        [Route("all/tree")]
        public IActionResult GetTree()
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var playlists = ITunesService.Instance.GetPlaylistCollectionTree();
            return Ok(playlists);
        }
    }
}