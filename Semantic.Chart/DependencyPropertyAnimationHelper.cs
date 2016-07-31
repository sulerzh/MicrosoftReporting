using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    internal static class DependencyPropertyAnimationHelper
    {
        public const int KeyFramesPerSecond = 20;
        private const string StoryboardKeyPattern = "__{0}__";

        internal static string GetStoryboardKey(string propertyPath)
        {
            return string.Format(CultureInfo.InvariantCulture, "__{0}__", new object[1] { propertyPath });
        }

        public static Storyboard CreateAnimation(this DependencyObject target, Dictionary<string, StoryboardInfo> storyboards, DependencyProperty animatingDependencyProperty, string propertyPath, string propertyKey, object initialValue, object targetValue, TimeSpan timeSpan, IEasingFunction easingFunction, Action releaseAction)
        {
            StoryboardInfo storyboardInfo;
            storyboards.TryGetValue(DependencyPropertyAnimationHelper.GetStoryboardKey(propertyKey), out storyboardInfo);
            if (storyboardInfo != null)
            {
                DependencyObject storyboardTarget = storyboardInfo.StoryboardTarget;
                storyboardInfo.Storyboard.Stop();
                storyboards.Remove(DependencyPropertyAnimationHelper.GetStoryboardKey(propertyKey));
                if (storyboardInfo.ReleaseAction != null)
                {
                    storyboardInfo.ReleaseAction();
                    storyboardInfo.ReleaseAction = null;
                }
            }
            storyboardInfo = new StoryboardInfo();
            storyboardInfo.Storyboard = target.CreateStoryboard(animatingDependencyProperty, propertyPath, propertyKey, ref targetValue, timeSpan, easingFunction);
            storyboardInfo.ReleaseAction = releaseAction;
            storyboardInfo.StoryboardTarget = target;
            storyboardInfo.AnimateFrom = initialValue;
            storyboardInfo.Storyboard.Completed += (source, args) =>
           {
               storyboards.Remove(DependencyPropertyAnimationHelper.GetStoryboardKey(propertyKey));
               if (storyboardInfo.ReleaseAction == null)
                   return;
               storyboardInfo.ReleaseAction();
               storyboardInfo.ReleaseAction = (Action)null;
           };
            storyboards.Add(DependencyPropertyAnimationHelper.GetStoryboardKey(propertyKey), storyboardInfo);
            return storyboardInfo.Storyboard;
        }

        private static Storyboard CreateStoryboard(this DependencyObject target, DependencyProperty animatingDependencyProperty, string propertyPath, string propertyKey, ref object toValue, TimeSpan durationTimeSpan, IEasingFunction easingFunction)
        {
            object obj = target.GetValue(animatingDependencyProperty);
            Storyboard storyboard = new Storyboard();
            Storyboard.SetTarget(storyboard, target);
            Storyboard.SetTargetProperty(storyboard, new PropertyPath(propertyPath, new object[0]));
            if (obj != null && toValue != null)
            {
                double doubleValue1;
                double doubleValue2;
                if (ValueHelper.TryConvert(obj, out doubleValue1) && ValueHelper.TryConvert(toValue, out doubleValue2))
                {
                    DoubleAnimation doubleAnimation = new DoubleAnimation();
                    doubleAnimation.Duration = durationTimeSpan;
                    doubleAnimation.To = new double?(ValueHelper.ToDouble(toValue));
                    toValue = doubleAnimation.To;
                    storyboard.Children.Add(doubleAnimation);
                }
                else
                {
                    DateTime dateTimeValue1;
                    DateTime dateTimeValue2;
                    if (ValueHelper.TryConvert(obj, out dateTimeValue1) && ValueHelper.TryConvert(toValue, out dateTimeValue2))
                    {
                        ObjectAnimationUsingKeyFrames animationUsingKeyFrames = new ObjectAnimationUsingKeyFrames();
                        animationUsingKeyFrames.Duration = durationTimeSpan;
                        long count = (long)(durationTimeSpan.TotalSeconds * 20.0);
                        if (count < 2L)
                            count = 2L;
                        IEnumerable<TimeSpan> intervalsInclusive = ValueHelper.GetTimeSpanIntervalsInclusive(durationTimeSpan, count);
                        foreach (DiscreteObjectKeyFrame discreteObjectKeyFrame in ValueHelper.GetDateTimesBetweenInclusive(dateTimeValue1, dateTimeValue2, count).Zip<DateTime, TimeSpan, DiscreteObjectKeyFrame>(intervalsInclusive, (dateTime, timeSpan) =>
                       {
                           return new DiscreteObjectKeyFrame() { Value = (object)dateTime, KeyTime = (KeyTime)timeSpan };
                       }))
                        {
                            animationUsingKeyFrames.KeyFrames.Add(discreteObjectKeyFrame);
                            toValue = discreteObjectKeyFrame.Value;
                        }
                        storyboard.Children.Add(animationUsingKeyFrames);
                    }
                }
            }
            if (storyboard.Children.Count == 0)
            {
                ObjectAnimationUsingKeyFrames animationUsingKeyFrames = new ObjectAnimationUsingKeyFrames();
                DiscreteObjectKeyFrame discreteObjectKeyFrame1 = new DiscreteObjectKeyFrame();
                discreteObjectKeyFrame1.Value = toValue;
                discreteObjectKeyFrame1.KeyTime = new TimeSpan(0, 0, 0);
                DiscreteObjectKeyFrame discreteObjectKeyFrame2 = discreteObjectKeyFrame1;
                animationUsingKeyFrames.KeyFrames.Add(discreteObjectKeyFrame2);
                storyboard.Children.Add(animationUsingKeyFrames);
            }
            return storyboard;
        }

        public static void SetValueWithAnimation(this DataPoint dataPoint, DependencyProperty dp, string propertyName, double value)
        {
            double num = (double)dataPoint.GetValue(dp);
            if (num == value)
                return;
            Series series = dataPoint.Series;
            if (series == null || series.ChartArea == null || (num.IsNaN() || value.IsNaN()))
                dataPoint.SetValue(dp, value);
            else
                DependencyPropertyAnimationHelper.BeginAnimation(series.ChartArea, propertyName, num, value, (current, next) => dataPoint.SetValue(dp, next), dataPoint.Storyboards, series.ActualTransitionDuration, series.ActualTransitionEasingFunction);
        }

        public static void BeginAnimation(ChartArea seriesHost, string propertyName, object currentValue, object targetValue, Action<object, object> propertyUpdateAction, Dictionary<string, StoryboardInfo> storyboards, TimeSpan timeSpan, IEasingFunction easingFunction)
        {
            if (timeSpan == TimeSpan.Zero)
                propertyUpdateAction(currentValue, targetValue);
            else
                DependencyPropertyAnimationHelper.CreateAnimation(seriesHost, propertyName, currentValue, targetValue, propertyUpdateAction, storyboards, timeSpan, easingFunction).Begin();
        }

        public static Storyboard CreateAnimation(ChartArea seriesHost, string propertyName, object currentValue, object targetValue, Action<object, object> propertyUpdateAction, Dictionary<string, StoryboardInfo> storyboards, TimeSpan timeSpan, IEasingFunction easingFunction)
        {
            ObjectPool<PropertyAnimator, object> propertyAnimatorPool = (ObjectPool<PropertyAnimator, object>)seriesHost.SingletonRegistry.GetSingleton("__PropertyAnimatorPool__", () => (object)new ObjectPool<PropertyAnimator, object>((Func<PropertyAnimator>)(() => new PropertyAnimator()), (Action<PropertyAnimator, object>)((obj, context) => obj.AnimatedValue = context), (Action<PropertyAnimator>)(obj => obj.UpdateAction = (Action<object, object>)null)), obj => ((ObjectPool<PropertyAnimator, object>)obj).ReleaseAll());
            PropertyAnimator propertyAnimator = propertyAnimatorPool.Get(currentValue);
            propertyAnimator.UpdateAction = propertyUpdateAction;
            Action releaseAction = () =>
           {
               propertyAnimatorPool.Release(propertyAnimator);
               propertyAnimatorPool.AdjustPoolSize();
           };
            return propertyAnimator.CreateAnimation(storyboards, PropertyAnimator.AnimatedValueProperty, "AnimatedValue", propertyName, currentValue, targetValue, timeSpan, easingFunction, releaseAction);
        }
    }
}
