using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Schober.Felix.ITunes.Controller;
using Schober.Felix.ITunes.Controller.Model;
using Schober.Felix.ITunes.Server.Model;

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
        [Route("folder/{folderName}")]
        public IActionResult GetFolder(string folderName)
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var requestedFolder = SearchFolder(folderName);

            if (requestedFolder == null) return NotFound(folderName);

            return Ok(requestedFolder);
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

        [HttpGet]
        [Route("folder/{folderName}/{index}")]
        public IActionResult GetPlaylistByFolderAndIndex(string folderName, int index)
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var requestedFolder = SearchFolder(folderName);

            if (requestedFolder == null) return NotFound(folderName);

            if (requestedFolder.Nodes < index)
            {
                return NotFound(new IntegerResponse()
                {
                    Name = "index out of bounds",
                    Data = index
                });
            }

            return Ok(requestedFolder.PlaylistNodes[index]);
        }

        [HttpGet]
        [Route("folder/{folderName}/{index}/cover")]
        public IActionResult GetPlaylistCoverByFolderAndIndex(string folderName, int index)
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var requestedFolder = SearchFolder(folderName);

            if (requestedFolder == null) return NotFound(folderName);

            if (requestedFolder.Nodes < index)
            {
                return NotFound(new IntegerResponse()
                {
                    Name = "index out of bounds",
                    Data = index
                });
            }

            var reqestedPlaylist = requestedFolder.PlaylistNodes[index].Playlist;
            if (reqestedPlaylist.Songs == 0)
            {
                throw new Exception($"The requested playlist with name {reqestedPlaylist.Name} does not contain any songs.");
            }

            //var requestedPlaylistFirstTrack = requestedFolder.PlaylistNodes[index].Playlist.TrackList[0];

            FileStream image;
            try
            {
                image  = ControllerServices.GetCoverForPlaylist(reqestedPlaylist);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not artwork");
                Console.WriteLine(e);
                return NotFound(e);
            }

            if (image == null) return NotFound(reqestedPlaylist);
            return File(image, "image/jpeg");
        }

        [HttpPost]
        [Route("folder/{folderName}/{index}/play")]
        public IActionResult PlayPlaylistFromFolder(string folderName, int index)
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var requestedFolder = SearchFolder(folderName);

            if (requestedFolder == null) return NotFound(folderName);

            if (requestedFolder.Nodes < index)
            {
                return NotFound(new IntegerResponse()
                {
                    Name = "index out of bounds",
                    Data = index
                });
            }

            var reqestedPlaylist = requestedFolder.PlaylistNodes[index].Playlist;
            if (reqestedPlaylist.Songs == 0)
            {
                throw new Exception($"The requested playlist with name {reqestedPlaylist.Name} does not contain any songs.");
            }

            var firstTrack = reqestedPlaylist.Play();
            return Ok(firstTrack);
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

        private PlaylistCollection SearchFolder(string folderName)
        {
            var playlists = ITunesService.Instance.GetPlaylistCollectionTree();
            return SearchFolder(folderName, playlists);
        }

        private PlaylistCollection SearchFolder(string folderName, PlaylistCollection currentNode)
        {
            if (folderName.Equals(currentNode.Name)) return currentNode;

            // search all children 
            foreach (var currentNodePlaylistNode in currentNode.PlaylistNodes)
            {
                var result = SearchFolder(folderName, currentNodePlaylistNode);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

    }
}