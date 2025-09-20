using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cointero
{
    internal class Status
    {
        private bool _autoFillUp;
        private enum _mode
        {
            Idle,
            Save,
            Run,
            Simul
        }
        static private _mode mode;

        private string _status;


        public int getStatus()
        {
            return ((int)mode);
        }

        public bool isRunMode()
        {
            if (mode == _mode.Run) return true;
            else return false;
        }

        public void setRunMode()
        {
            mode = _mode.Run;
        }

        public bool isIdleMode()
        {
            if (mode == _mode.Idle) return true;
            else return false;
        }

        public void setIdleMode()
        {
             mode = _mode.Idle;
        }


        public bool isSaveMode()
        {
            if (mode == _mode.Save) return true;
            else return false;
        }

        public void setSaveMode()
        {
            mode = _mode.Save;
        }

        public bool isSimulMode()
        {
            if (mode == _mode.Simul) return true;
            else return false;
        }

        public void setSimulMode()
        {
            mode = _mode.Simul;
        }

        public bool isAutoFillUp()
        {
            return _autoFillUp;
        }

        public void setAutoFillUp()
        {
            _autoFillUp = true;
        }

        public void clrAutoFillUp()
        {
            _autoFillUp = false;
        }



    }
}
