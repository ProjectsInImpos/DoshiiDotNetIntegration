using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Models;

namespace DoshiiDotNetIntegration.Interfaces
{
    /// <summary>
    /// there are currently no socket messages for doshii apps, this interface is only used when the socket connection is initially opened to sync the doshii apps with the apps stored on the pos. 
    /// </summary>
    public interface IAppManager
    {
        /// <summary>
        /// This method should retreive all the doshii apps from the pos. 
        /// </summary>
        /// <param name="DoshiiMemberId"></param>
        /// <returns></returns>
        IEnumerable<Models.App> GetAppsFromPos();

        /// <summary>
        /// This method should create a doshii apps on the pos
        /// </summary>
        /// <param name="newMember"></param>
        /// <returns></returns>
        bool CreateAppOnPos(App newMember);

        /// <summary>
        /// This method should update a doshii apps of the pos. 
        /// </summary>
        /// <param name="updatedMember"></param>
        /// <returns></returns>
        bool UpdateAppOnPos(App updatedMember);

        /// <summary>
        /// This method should update a doshii apps of the pos. 
        /// </summary>
        /// <param name="updatedMember"></param>
        /// <returns></returns>
        bool DeleteAppOnPos(App deletedMember);
    }
}
