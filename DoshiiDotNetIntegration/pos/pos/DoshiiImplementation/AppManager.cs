using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.Interfaces;
using DoshiiDotNetIntegration.Models;

namespace pos.DoshiiImplementation
{
    public class AppManager : IAppManager
    {
        public IEnumerable<App> GetAppsFromPos()
        {
            return LiveData.AppsList;
        }

        public void CreateAppOnPos(App newApp)
        {
            LiveData.AppsList.Add(newApp);
        }

        public void UpdateAppOnPos(App updatedApp)
        {
            foreach (var app in LiveData.AppsList.Where(a => a.Id == updatedApp.Id))
            {
                app.AppMember = updatedApp.AppMember;
                app.Name = updatedApp.Name;
                app.Points = updatedApp.Points;
                app.Ref = updatedApp.Ref;
                app.Types = updatedApp.Types;
            }
        }

        public void DeleteAppOnPos(App deletedApp)
        {
            LiveData.AppsList.RemoveAll(x => x.Id == deletedApp.Id);
        }
    }
}
