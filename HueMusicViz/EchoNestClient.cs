using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace HueMusicViz
{
    class EchoNestClient
    {
        private readonly WebClient _webClient;
        private string _apiKey;

        public EchoNestClient()
        {
            _webClient = new WebClient();
            setup();
        }

        private async void setup()
        {
            await _refreshSpotifyAPIKey();
        }

        private async Task<bool> _refreshSpotifyAPIKey()
        {
            var url = "https://accounts.spotify.com/api/token";
#if DEBUG
            // sometimes locally i use Charles and this is SSL so it's just safer okay
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback((object sender, X509Certificate cert, X509Chain chain, System.Net.Security.SslPolicyErrors error) => { return true; });
#endif
            _webClient.Headers["Authorization"] = "Basic " + _base64(Secrets.SPOTIFY_CLIENT_ID + ":" + Secrets.SPOTIFY_CLIENT_SECRET);
            _webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var response_string = await _webClient.UploadStringTaskAsync(new Uri(url), "grant_type=client_credentials");
            var response = _andParse<SpotifyAPITokenJson>(response_string);

            // #TODO: save the response.expires_in so we know when we need to get a new token
            // note that setting these headers sets it for all other calls, which is exactly what we want
            _webClient.Headers["Authorization"] = "Bearer " + response.access_token;
            _webClient.Headers[HttpRequestHeader.ContentType] = "";
            _webClient.Headers[HttpRequestHeader.Accept] = "application/json";
             
            return true;
        }

        public async Task<EchoNestAudioFeature> getSongSummary(string spotifyID)
        {
            var url = string.Format("https://api.spotify.com/v1/audio-features?ids={0}", spotifyID);
            var response = await _downloadAndParse<EchoNestJSONWrapper>(url);

            if (response.audio_features.Any())
            {
                return response.audio_features.First();
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

        private string _base64(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        private async Task<T> _downloadAndParse<T>(string url)
        {
            var jsonString = await _webClient.DownloadStringTaskAsync(url);

            return _andParse<T>(jsonString);
        }

        public T _andParse<T>(string data)
        {
            using (var sr = new StringReader(data))
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
        public EchoNestAudioFeature[] audio_features { get; set;  }
    }

    class EchoNestAudioFeature
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

    class SpotifyAPITokenJson
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
