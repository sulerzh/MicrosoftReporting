using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    public class HighContrastBlackAndWhiteEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(HighContrastBlackAndWhiteEffect), 0);
        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register("Amount", typeof(double), typeof(HighContrastBlackAndWhiteEffect), new PropertyMetadata(0.5, ShaderEffect.PixelShaderConstantCallback(0)));
        public static readonly DependencyProperty InvertProperty = DependencyProperty.Register("Invert", typeof(double), typeof(HighContrastBlackAndWhiteEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(1)));

        public Brush Input
        {
            get
            {
                return (Brush)this.GetValue(HighContrastBlackAndWhiteEffect.InputProperty);
            }
            set
            {
                this.SetValue(HighContrastBlackAndWhiteEffect.InputProperty, value);
            }
        }

        public double Amount
        {
            get
            {
                return (double)this.GetValue(HighContrastBlackAndWhiteEffect.AmountProperty);
            }
            set
            {
                this.SetValue(HighContrastBlackAndWhiteEffect.AmountProperty, value);
            }
        }

        public double Invert
        {
            get
            {
                return (double)this.GetValue(HighContrastBlackAndWhiteEffect.InvertProperty);
            }
            set
            {
                this.SetValue(HighContrastBlackAndWhiteEffect.InvertProperty, value);
            }
        }

        public HighContrastBlackAndWhiteEffect()
        {
            this.PixelShader = new PixelShader()
            {
                UriSource = new Uri("/Microsoft.Reporting.Common.Toolkit;component/HighContrast/HighContrastBlackAndWhite.ps", UriKind.Relative)
            };
            this.UpdateShaderValue(HighContrastBlackAndWhiteEffect.InputProperty);
            this.UpdateShaderValue(HighContrastBlackAndWhiteEffect.AmountProperty);
            this.UpdateShaderValue(HighContrastBlackAndWhiteEffect.InvertProperty);
        }
    }
}
