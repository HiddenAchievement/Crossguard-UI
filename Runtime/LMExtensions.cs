using System;
using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace HiddenAchievement.CrossguardUi
{
    public static class LMExtensions
    {
        public static MotionHandle BindToColorNoAlpha<TOptions, TAdapter>(this MotionBuilder<Color, TOptions, TAdapter> builder, Graphic graphic)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Color, TOptions>
        {
            if (graphic == null) throw new ArgumentNullException(nameof(graphic));
            return builder.Bind(graphic, static (x, target) =>
            {
                x.a = target.color.a;
                target.color = x;
            });
        }
        
        public static MotionHandle BindToColor<TOptions, TAdapter>(this MotionBuilder<Color, TOptions, TAdapter> builder, CanvasRenderer renderer)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Color, TOptions>
        {
            if (renderer == null) throw new ArgumentNullException(nameof(renderer));
            return builder.Bind(renderer, static (x, target) =>
            {
                target.SetColor(x);
            });
        }
        
        public static MotionHandle BindToColorA<TOptions, TAdapter>(this MotionBuilder<float, TOptions, TAdapter> builder, CanvasRenderer renderer)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<float, TOptions>
        {
            if (renderer == null) throw new ArgumentNullException(nameof(renderer));
            return builder.Bind(renderer, static (x, target) =>
            {
                Color c = target.GetColor();
                c.a = x;
                target.SetColor(c);
            });
        }
        
        public static MotionHandle BindToColorNoAlpha<TOptions, TAdapter>(this MotionBuilder<Color, TOptions, TAdapter> builder, CanvasRenderer renderer)
            where TOptions : unmanaged, IMotionOptions
            where TAdapter : unmanaged, IMotionAdapter<Color, TOptions>
        {
            if (renderer == null) throw new ArgumentNullException(nameof(renderer));
            return builder.Bind(renderer, static (x, target) =>
            {
                x.a = target.GetColor().a;
                target.SetColor(x);
            });
        }
    }
}