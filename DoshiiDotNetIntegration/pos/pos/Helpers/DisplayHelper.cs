using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Enums;

namespace pos.Helpers
{
    public class DisplayHelper
    {
        public DisplayHelper(PosFunctions posView)
        {
            this.posView = posView;
        }

        public PosFunctions posView;
        
        public void WriteToLog(string message, DoshiiLogLevels level)
        {
            Color textColour = Color.Black;
            switch (level)
            {
                case DoshiiLogLevels.Debug:
                    textColour = Color.Black;
                    break;
                case DoshiiLogLevels.Error:
                    textColour = Color.Red;
                    break;
                case DoshiiLogLevels.Fatal:
                    textColour = Color.DarkRed;
                    break;
                case DoshiiLogLevels.Info:
                    textColour = Color.Blue;
                    break;
                case DoshiiLogLevels.Warning:
                    textColour = Color.Orange;
                    break;
            }
            posView.WriteToLog(message, textColour, Color.White, FontStyle.Regular);
            DataPersistanceHelper.WriteLogTofile(message);
        }
    }
}
