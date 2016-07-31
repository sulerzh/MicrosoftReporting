using Microsoft.Reporting.Windows.Common.Internal;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

namespace Microsoft.Reporting.Windows.Chart.Internal
{
    public class StoryboardGroup
    {
        private int _runningStoryboards;

        public Collection<Storyboard> Children { get; private set; }

        public event EventHandler Completed;

        public StoryboardGroup()
        {
            this.Children = new Collection<Storyboard>();
        }

        public void Begin()
        {
            this._runningStoryboards = this.Children.Count;
            this.Children.ForEachWithIndex<Storyboard>((item, index) =>
           {
               item.Completed += (EventHandler)((source, args) =>
               {
                   item.Stop();
                   --this._runningStoryboards;
                   if (this._runningStoryboards != 0 || this.Completed == null)
                       return;
                   this.Completed((object)this, EventArgs.Empty);
               });
               item.Begin();
           });
        }

        public void Stop()
        {
            this.Children.ForEachWithIndex<Storyboard>((item, index) => item.Stop());
            this._runningStoryboards = 0;
        }
    }
}
