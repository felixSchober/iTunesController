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

        [HttpGet]
        [Route("all/list")]
        public IActionResult GetAllList()
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }


            var playlists = ITunesService.Instance.GetPlaylists();
            return Ok(playlists);
        }

        [HttpGet]
        [Route("{sourceid}/{playlistId}")]
        public IActionResult GetPlaylist(int sourceId, int playlistId)
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var playlist = ITunesService.Instance.GetPlaylistById(sourceId, playlistId);

            if (playlist == null) return NotFound(playlistId);
            return Ok(playlist);
        }

        [HttpPost]
        [Route("{sourceid}/{playlistId}/play")]
        public IActionResult PlayPlaylist(int sourceId, int playlistId)
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var playlist = ITunesService.Instance.GetPlaylistById(sourceId, playlistId);

            if (playlist == null) return NotFound(playlistId);

            var firstTrack = playlist.Play();
            return Ok(firstTrack);
        }
    }
}