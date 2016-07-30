using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    public class MonochromeEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(MonochromeEffect), 0);
        public static readonly DependencyProperty InvertProperty = DependencyProperty.Register("Invert", typeof(double), typeof(MonochromeEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(0)));

        public Brush Input
        {
            get
            {
                return (Brush)this.GetValue(MonochromeEffect.InputProperty);
            }
            set
            {
                this.SetValue(MonochromeEffect.InputProperty, value);
            }
        }

        public double Invert
        {
            get
            {
                return (double)this.GetValue(MonochromeEffect.InvertProperty);
            }
            set
            {
                this.SetValue(MonochromeEffect.InvertProperty, value);
            }
        }

        public MonochromeEffect()
        {
            this.PixelShader = new PixelShader()
            {
                UriSource = new Uri("/Microsoft.Reporting.Common.Toolkit;component/HighContrast/Monochrome.ps", UriKind.Relative)
            };
            this.UpdateShaderValue(MonochromeEffect.InputProperty);
            this.UpdateShaderValue(MonochromeEffect.InvertProperty);
        }
    }
}
