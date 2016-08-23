# HueMusicViz
trying to make my Hue lights dance to the beat

[![](https://scontent-atl3-1.cdninstagram.com/t51.2885-15/e15/11374102_641545675981910_523380368_n.jpg?ig_cache_key=MTA0NTMyMDQ1NjEwODI0MDI3OQ%3D%3D.2)](https://www.instagram.com/p/6BuWqex-2X/)

Click video for a quick demo of an older build.

This works by using the Echo Nest's audio analysis (see [the API](https://developer.spotify.com/web-api/get-several-audio-features/), the [acoustic attributes description](http://developer.echonest.com/acoustic-attributes.html), and the [analysis_url spec](http://developer.echonest.com/docs/v4/_static/AnalyzeDocumentation.pdf)) to get a closest approximation of the measures and beats in the song. When a new song is played on Spotify, we get those then wait for it to hit the right time to update the lights, changing the color and intensity of them per some of the song's attributes.

If you are running Windows with dev tooling, use Spotify, and have Hue lights (full color, not the shades of white ones), I'd love for you to try this. If you play the music on your headphones and walk around your living space it's like a silent rave. Or you can cast the sound to speakers or anything and make it a real rave (but if you try, before you tell me it's awful see the notes on calibrating).

## Setup:

- Checkout this repository (and submodules) and open it in Visual Studio (sorry)
- Make a `Secrets.cs` based on [`Secrets.cs.sample`](https://github.com/jc4p/HueMusicViz/blob/659ac24a4929d4865bd4ffb27354c6a387745f24/HueMusicViz/Secrets.cs.sample) and add it to your solution.
- Create an app with the [Spotify developer API](https://developer.spotify.com/) and add the secrets in that file.
- Modify some values in MainForm.cs:
  - [`lights`](https://github.com/jc4p/HueMusicViz/blob/659ac24a4929d4865bd4ffb27354c6a387745f24/HueMusicViz/MainForm.cs#L32) is a list of the IDs of the specific lights you want to dance (they're a direct match to the numbers the Hue app on your phone shows for them)
  - [`bridgeIp`](https://github.com/jc4p/HueMusicViz/blob/659ac24a4929d4865bd4ffb27354c6a387745f24/HueMusicViz/MainForm.cs#L57) is the IP of your Hue Bridge (If you don't know this maybe you can find a list of local IPs on your router's config page)
  - [`hueAppKey`](https://github.com/jc4p/HueMusicViz/blob/659ac24a4929d4865bd4ffb27354c6a387745f24/HueMusicViz/MainForm.cs#L73) is your Hue API key (See the first three steps of [this Hue guide](http://www.developers.meethue.com/documentation/getting-started)). Note that I don't hide my own appKey in `Secrets.cs` since I think it's just a local network API key so who cares.

The good news is after you set everything you don't really need to change anything again, unless you want to modify which lights or get some new ones.

## To Run:

- Open Spotify (if it's not open before the app launches the app will crash, probably)
- Run the app from Visual Studio (either in Debug or Release mode)
- Put on a new song on (the app listens for "Song Change" events to kick in so...)
- Enjoy!

If you put on a fast intense angry song the lights should go to a dim dark red, if you put on a happy song they should go and pink-ish and bright.

## Notes:

I think the code is relatively readable, it's not the cleanest code but I don't really know C# so I'm not too upset about it. Everything you could possibly care about (my application logic wise) is in `MainForm.cs`.

Honestly the only fun part is the [code that sets the colors the lights will toggle between](https://github.com/jc4p/HueMusicViz/blob/cfd4e85f43831bbf0eea407c75f05a1a9a89024c/HueMusicViz/MainForm.cs#L143) and even that is pretty simple. Feel free to change the base [`HUE_MIN`](https://github.com/jc4p/HueMusicViz/blob/659ac24a4929d4865bd4ffb27354c6a387745f24/HueMusicViz/MainForm.cs#L38) and [`HUE_MAX`](https://github.com/jc4p/HueMusicViz/blob/659ac24a4929d4865bd4ffb27354c6a387745f24/HueMusicViz/MainForm.cs#L39) if "high energy" doesn't mean "dark red" to you and "low energy" doesn't mean "blue". Or change the entire method, the world is your oyster.

I tried using FFTs and local audio loopback devices in older versions (it's either in this git history or in a different repo, I'm not sure), but Echo Nest was just a lot easier and more accurate (I think). It does limit me to Spotify only, but Spotify has mostly everything I listen to so it's okay.

I also tried doing lots of color variations based on different writing on it, the best ones were Scriabin's color scale, but I'm not the kind of person to settle with some matrices someone wrote before World War I, you know?

## On calibration:

There's a lot of percise timing to get this right, even more when you introduce streaming/casting/any extra delay.

To calibrate for your local latency, you should play around with the `FeedbackTester`, to do this change `Program.cs` to call `FeedbackTester()` not `MainForm()`, and modify `FeedbackTester.cs` to point at your hue bridge and lights (same as the setup for `MainForm`).

The calibration tool works in two steps. Hit the "TOGGLE LIGHTS" button, then the milliseconds (or as fast as you can, I guess) all the lights you told it to modify change color, hit space. Do this ten times, or until you get bored, then come up with a reasonable average or median value. I'm not a scientist I don't know, you'll have a list of delay times and you can just pick something.

Once you get a reasonable estimation of your local latency, update the value of `LIGHTS_DELAY_MS` in `MainForm` and move `Program.cs` back to calling `MainForm()`.
