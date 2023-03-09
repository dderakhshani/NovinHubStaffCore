using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovinDevHubStaffCore.Core.Activities
{
    public class MouseKeyboardActivity
    {
        //Type 1=Keyboard,2=Mouse,3=Overall
        public short Type { get; set; }
        public System.DateTime StartActiveTime { get; set; }
        public System.DateTime EndActiveTime { get; set; }

        public TimeSpan OverlapPeriod(MouseKeyboardActivity period2)
        {
            //if current time is start before period2 and have overlap
            if (StartActiveTime < period2.StartActiveTime && period2.StartActiveTime < EndActiveTime && EndActiveTime < period2.EndActiveTime)
            {
                return EndActiveTime - period2.StartActiveTime;
            }
            //if period2 is start before current time and have overlap
            if (period2.StartActiveTime < StartActiveTime && StartActiveTime < period2.EndActiveTime && period2.EndActiveTime < EndActiveTime)
            {
                return period2.EndActiveTime - StartActiveTime;
            }
            //if period2 contains current time
            else if (period2.StartActiveTime < StartActiveTime && EndActiveTime < period2.EndActiveTime)
            {
                return EndActiveTime - StartActiveTime;
            }
            //if   current time contains period
            else if (StartActiveTime < period2.StartActiveTime && period2.EndActiveTime < EndActiveTime)
            {
                return period2.EndActiveTime - period2.StartActiveTime;
            }
            else
                return TimeSpan.FromSeconds(0);
        }

    }
}
