using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Microsoft.Reporting.Common.Toolkit.Internal
{
    public class HighContrastInvertColorsEffect : ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(HighContrastInvertColorsEffect), 0);
        public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register("Brightness", typeof(double), typeof(HighContrastInvertColorsEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(0)));
        public static readonly DependencyProperty ContrastProperty = DependencyProperty.Register("Contrast", typeof(double), typeof(HighContrastInvertColorsEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(1)));
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.Register("Enabled", typeof(double), typeof(HighContrastInvertColorsEffect), new PropertyMetadata(1.0, ShaderEffect.PixelShaderConstantCallback(2)));
        public static readonly DependencyProperty InvertProperty = DependencyProperty.Register("Invert", typeof(double), typeof(HighContrastInvertColorsEffect), new PropertyMetadata(0.0, ShaderEffect.PixelShaderConstantCallback(3)));

        public Brush Input
        {
            get
            {
                return (Brush)this.GetValue(HighContrastInvertColorsEffect.InputProperty);
            }
            set
            {
                this.SetValue(HighContrastInvertColorsEffect.InputProperty, value);
            }
        }

        public double Brightness
        {
            get
            {
                return (double)this.GetValue(HighContrastInvertColorsEffect.BrightnessProperty);
            }
            set
            {
                this.SetValue(HighContrastInvertColorsEffect.BrightnessProperty, value);
            }
        }

        public double Contrast
        {
            get
            {
                return (double)this.GetValue(HighContrastInvertColorsEffect.ContrastProperty);
            }
            set
            {
                this.SetValue(HighContrastInvertColorsEffect.ContrastProperty, value);
            }
        }

        public double Enabled
        {
            get
            {
                return (double)this.GetValue(HighContrastInvertColorsEffect.EnabledProperty);
            }
            set
            {
                this.SetValue(HighContrastInvertColorsEffect.EnabledProperty, value);
            }
        }

        public double Invert
        {
            get
            {
                return (double)this.GetValue(HighContrastInvertColorsEffect.InvertProperty);
            }
            set
            {
                this.SetValue(HighContrastInvertColorsEffect.InvertProperty, value);
            }
        }

        public HighContrastInvertColorsEffect()
        {
            this.PixelShader = new PixelShader()
            {
                UriSource = new Uri("/Microsoft.Reporting.Common.Toolkit;component/HighContrast/HighContrastInvertColors.ps", UriKind.Relative)
            };
            this.UpdateShaderValue(HighContrastInvertColorsEffect.InputProperty);
            this.UpdateShaderValue(HighContrastInvertColorsEffect.BrightnessProperty);
            this.UpdateShaderValue(HighContrastInvertColorsEffect.ContrastProperty);
            this.UpdateShaderValue(HighContrastInvertColorsEffect.EnabledProperty);
            this.UpdateShaderValue(HighContrastInvertColorsEffect.InvertProperty);
        }
    }
}
