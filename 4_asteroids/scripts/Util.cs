using Godot;
using System;
using System.Timers;

namespace AsteroidsNamespce
{
    public class TimerPlus
    {
        private System.Timers.Timer timer;
        private DateTime startTime;
        private TimeSpan period;
        public bool isEnabled
        {
            get { return this.timer.Enabled; }
        }

        public TimerPlus(int durationMS, Action callback = null)
        {
            this.timer = new System.Timers.Timer();
            this.timer.Elapsed += new ElapsedEventHandler((sender, args) =>
            {
                if(callback != null)
                    callback.Invoke();
                this.timer.Stop(); 
            });
            this.timer.Interval = durationMS; 
            this.period = new TimeSpan(durationMS * 10_000);
        }

        public float getCompletionRatio()
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            double ratio = elapsed.TotalSeconds / period.TotalSeconds;
            return Math.Clamp((float)ratio, 0f, 1f);
        }

        public bool isRunning()
        {
            return this.timer.Enabled; 
        }


        public void Start()
        {
            this.timer.Start();
            this.startTime = DateTime.Now;
        }

        public void Stop()
        {
            this.timer.Stop();
        }
    }

}