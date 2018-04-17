using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Schober.Felix.ITunes.Controller;

namespace iTunesServer.Controllers
{
    public class PlayerController : ApiController
    {
        [HttpPut]
        [Route("api/player/pause")]
        public IHttpActionResult PutPause()
        {
            ITunesService.Instance.Pause();
            return Ok();
        }

        [HttpPut]
        [Route("api/player/play")]
        public IHttpActionResult PutPlay()
        {
            ITunesService.Instance.Play();
            return Ok();
        }

        [HttpPut]
        [Route("api/player/mute")]
        public IHttpActionResult PutMute()
        {
            ITunesService.Instance.ToggleMute();
            return Ok(ITunesService.Instance.IsMute);
        }
    }
}
