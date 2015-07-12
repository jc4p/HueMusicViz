using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueMusicViz.Models
{
    class Bar : EchoNestBar
    {
        public IEnumerable<EchoNestBeat> beats { get; set; }

        public Bar(EchoNestBar bar, IEnumerable<EchoNestBeat> beats)
        {
            this.start = bar.start;
            this.confidence = bar.confidence;
            this.duration = bar.duration;

            this.beats = beats;
        }

        public static IEnumerable<Bar> getBarsFromAnalysis(EchoNestAnalysisResponse analysis)
        {
            List<Bar> bars = new List<Bar>();

            foreach (var bar in analysis.bars)
            {
                IEnumerable<EchoNestBeat> beats = getBeatsInBar(analysis.beats, bar.start, bar.start + bar.duration);
                bars.Add(new Bar(bar, beats));
            }

            return bars;
        }

        private static IEnumerable<EchoNestBeat> getBeatsInBar(IEnumerable<EchoNestBeat> beats, double barStart, double barEnd)
        {
            List<EchoNestBeat> result = new List<EchoNestBeat>();

            foreach (var b in beats)
            {
                if (b.start >= barEnd)
                    break;

                if (b.start >= barStart)
                    result.Add(b);
            }

            return result;
        }
    }
}
