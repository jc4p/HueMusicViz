using CSCore.Utils;
namespace WinformsVisualization.Visualization
{
    public interface ISpectrumProvider
    {
        bool GetFftData(float[] fftBuffer, object context);
        bool GetFftDataComplex(Complex[] fftBuffer, object context);
        int GetFftBandIndex(float frequency);
    }
}