using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schober.Felix.ITunes.Controller;
using Schober.Felix.ITunes.Controller.Model;
using Schober.Felix.ITunes.Server.Model;

namespace iTunesServer.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PlayerController : Controller
    {

        #region GET States

        [HttpGet]
        public BooleanResponse GetState()
        {
            var response = new BooleanResponse
            {
                Data = ITunesService.Instance.IsActive,
                Name = nameof(ITunesService.Instance.IsActive)
            };
            return response;
        }

        [HttpGet]
        [Route("state/playPause")]
        public BooleanResponse GetPlayPauseState()
        {
            var response = new BooleanResponse
            {
                Data = ITunesService.Instance.IsPlaying,
                Name = nameof(ITunesService.Instance.IsPlaying)
            };
            return response;
        }

        [HttpGet]
        [Route("state/mute")]
        public BooleanResponse GetMuteState()
        {
            var response = new BooleanResponse
            {
                Data = ITunesService.Instance.IsMute,
                Name = nameof(ITunesService.Instance.IsMute)
            };
            return response;
        }

        [HttpGet]
        [Route("state/volume")]
        public IntegerResponse GetVolumeState()
        {
            var response = new IntegerResponse()
            {
                Data = ITunesService.Instance.Volume,
                Name = nameof(ITunesService.Instance.Volume)
            };
            return response;
        }

        #endregion

        #region POST States

        [HttpPost]
        [Route("state/playPause")]
        public IActionResult PostPlayPauseState()
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            ITunesService.Instance.TogglePlayPause();

            var response = new BooleanResponse
            {
                Data = ITunesService.Instance.IsPlaying,
                Name = nameof(ITunesService.Instance.IsPlaying)
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("state/mute")]
        public IActionResult PostMuteState()
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            ITunesService.Instance.ToggleMute();

            var response = new BooleanResponse
            {
                Data = ITunesService.Instance.IsMute,
                Name = nameof(ITunesService.Instance.IsMute)
            };
            return Ok(response);
        }

        [HttpPost]
        [Route("state/volume")]
        public IActionResult PostVolumeState([FromBody]IntegerRequest volumeParameter)
        {
            if (volumeParameter.Name == null || !volumeParameter.Name.Equals("Volume"))
            {
                return BadRequest(volumeParameter);
            }

            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var newVolume = ITunesService.Instance.ChangeVolume(volumeParameter.Data);
            var volumeValue = newVolume ?? 0;
            var response = new IntegerResponse()
            {
                Data = volumeValue,
                Name = nameof(ITunesService.Instance.Volume)
            };
            return Ok(response);
        }

        #endregion

        #region Player Controls

        [HttpPost]
        [Route("next")]
        public IActionResult NextTrack()
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }

            var nextTrack = ITunesService.Instance.SkipTrack();
            return Ok(nextTrack);
        }

        [HttpPost]
        [Route("previous")]
        public IActionResult PreviousTrack()
        {
            if (!ITunesService.Instance.IsActive)
            {
                return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
            }
            var prevTrack = ITunesService.Instance.PreviousTrack();
            return Ok(prevTrack);
        }


        #endregion

    }
}