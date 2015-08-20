using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HueMusicViz
{
    class EchoNestClient
    {
        private readonly WebClient _webClient;

        public EchoNestClient()
        {
            _webClient = new WebClient();
        }

        public async Task<EchoNestAudioSummary> getSongSummary(string spotifyURI)
        {
            var url = String.Format("https://developer.echonest.com/api/v4/track/profile?api_key={0}&id={1}&bucket=audio_summary", Secrets.ECHO_NEST_API_KEY, spotifyURI);
            var response = await _downloadAndParse<EchoNestJSONWrapper>(url);

            if (response.response.track != null)
            {
                return response.response.track.audio_summary;
            }
            return null;
        }

        public async Task<EchoNestAnalysisResponse> getAnalysis(string analysisUrl)
        {
            return await _downloadAndParse<EchoNestAnalysisResponse>(analysisUrl);
        }

        public async Task<IEnumerable<EchoNestBeat>> getBeats(string analysisUrl)
        {
            var response = await _downloadAndParse<EchoNestAnalysisResponse>(analysisUrl);
            return response.beats;
        }

        public async Task<IEnumerable<EchoNestBar>> getBars(string analysisUrl)
        {
            var response = await _downloadAndParse<EchoNestAnalysisResponse>(analysisUrl);
            return response.bars;
        }

        public async Task<IEnumerable<EchoNestTatum>> getTatums(string analysisUrl)
        {
            var response = await _downloadAndParse<EchoNestAnalysisResponse>(analysisUrl);
            return response.tatums;
        }

        private async Task<T> _downloadAndParse<T>(string url)
        {
            var jsonString = await _webClient.DownloadStringTaskAsync(url);

            using (var sr = new StringReader(jsonString))
            using (var jr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();
                var response = js.Deserialize<T>(jr);

                return response;
            }
        }

    }

    class EchoNestJSONWrapper
    {
        public EchoNestResponse response { get; set;  }
    }

    class EchoNestResponse
    { 
        public EchoNestTrack track { get; set; }
    }

    class EchoNestTrack
    {
        public EchoNestAudioSummary audio_summary { get; set; } 
    }

    class EchoNestAudioSummary
    {
        public double energy { get; set; }
        public double valence { get; set; }
        public double danceability { get; set; }
        public string analysis_url { get; set; }
    }

    class EchoNestAnalysisResponse
    {
        public IEnumerable<EchoNestBeat> beats { get; set; }
        public IEnumerable<EchoNestBar> bars { get; set; }
        public IEnumerable<EchoNestTatum> tatums { get; set; }
    }

    class EchoNestCommon
    {
        public double start { get; set; }
        public double duration { get; set; }
        public double confidence { get; set; }
    }

    class EchoNestTatum : EchoNestCommon
    { }

    class EchoNestBar : EchoNestCommon
    { }

    class EchoNestBeat : EchoNestCommon
    { }
}
