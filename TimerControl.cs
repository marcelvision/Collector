using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cointero
{
    class TimerControl
    {
        public System.Timers.Timer timer = new System.Timers.Timer(20);
        public void Create()
        {

        }

        public void Dispose()
        {
            timer.Dispose();
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }

    class TimerControlImage
    {
        public System.Timers.Timer timer = new System.Timers.Timer(20);
        public void Create()
        {

        }

        public void Dispose()
        {
            timer.Dispose();
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }

    class TimerControlRefresh
    {
        public System.Timers.Timer timer = new System.Timers.Timer(500);
        public void Create()
        {

        }

        public void Dispose()
        {
            timer.Dispose();
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }

    class TimerControlSimulator
    {
        public System.Timers.Timer timer = new System.Timers.Timer(1000);
        public void Create()
        {

        }

        public void Dispose()
        {
            timer.Dispose();
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }


}
