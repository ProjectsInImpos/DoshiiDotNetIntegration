using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration.CommunicationLogic;
using DoshiiDotNetIntegration.Enums;
using DoshiiDotNetIntegration.Helpers;
using DoshiiDotNetIntegration.Models;
using DoshiiDotNetIntegration.Models.ActionResults;

namespace DoshiiDotNetIntegration.Controllers
{
    internal class AppController
    {
        /// <summary>
        /// prop for the local <see cref="ControllersCollection"/> instance. 
        /// </summary>
        internal Models.ControllersCollection _controllersCollection;

        /// <summary>
        /// prop for the local <see cref="HttpController"/> instance.
        /// </summary>
        internal HttpController _httpComs;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controllerCollection"></param>
        /// <param name="httpComs"></param>
        internal AppController(Models.ControllersCollection controllerCollection, HttpController httpComs)
        {
            if (controllerCollection == null)
            {
                throw new NullReferenceException("controller cannot be null");
            }
            _controllersCollection = controllerCollection;
            if (_controllersCollection.LoggingController == null)
            {
                throw new NullReferenceException("doshiiLogger cannot be null");
            }
            if (httpComs == null)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, " Initialization failed - httpComs cannot be null");
                throw new NullReferenceException("httpComs cannot be null");
            }
            _httpComs = httpComs;

        }

        internal virtual ObjectActionResult<List<App>> GetApps()
        {
            try
            {
                return _httpComs.GetApps();
            }
            catch (Exceptions.RestfulApiErrorResponseException rex)
            {
                throw rex;
            }
        }

        /// <summary>
        /// This method will return a failed result when any of the apps that are to be syncronised fail in the sync process, the fail reason in the ActionResultBasic will 
        /// give details about which apps failed to sync. 
        /// </summary>
        /// <returns></returns>
        internal virtual ActionResultBasic SyncDoshiiAppsWithPosApps()
        {
            try
            {
                StringBuilder failedReasonBuilder = new StringBuilder();
                List<App> DoshiiMembersList = GetApps().ReturnObject;
                List<App> PosMembersList = _controllersCollection.AppManager.GetAppsFromPos().ToList();

                var doshiiAppsHashSet = new HashSet<string>(DoshiiMembersList.Select(p => p.Id));
                var posAppsHashSet = new HashSet<string>(PosMembersList.Select(p => p.Id));

                var appsNotInDoshii = PosMembersList.Where(p => !doshiiAppsHashSet.Contains(p.Id));
                foreach (var mem in appsNotInDoshii)
                {
                    try
                    {
                        _controllersCollection.AppManager.DeleteAppOnPos(mem);
                    }
                    catch (Exception ex)
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception deleting an app on the pos with doshii memberId {0}", mem.Id), ex);
                        failedReasonBuilder.AppendLine(string.Format("App with Id {0} failed to delete from the pos",
                            mem.Id));
                    }

                }

                var membersInPos = DoshiiMembersList.Where(p => posAppsHashSet.Contains(p.Id));
                foreach (var mem in membersInPos)
                {
                    App posMember = PosMembersList.FirstOrDefault(p => p.Id == mem.Id);
                    if (!mem.Equals(posMember))
                    {
                        try
                        {
                            _controllersCollection.AppManager.UpdateAppOnPos(mem);
                        }
                        catch (Exception ex)
                        {
                            _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception updating an app on the pos with doshii memberId {0}", mem.Id), ex);
                            failedReasonBuilder.AppendLine(string.Format("App with Id {0} failed to update on the pos",
                            mem.Id));
                        }

                    }
                }

                var membersNotInPos = DoshiiMembersList.Where(p => !posAppsHashSet.Contains(p.Id));
                foreach (var mem in membersNotInPos)
                {
                    try
                    {
                        _controllersCollection.AppManager.CreateAppOnPos(mem);
                    }
                    catch (Exception ex)
                    {
                        _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception creating a member on the pos with doshii memberId {0}", mem.Id), ex);
                        failedReasonBuilder.AppendLine(string.Format("App with Id {0} failed to create on the pos",
                            mem.Id));
                    }

                }

                if (string.IsNullOrEmpty(failedReasonBuilder.ToString()))
                {
                    return new ActionResultBasic()
                    {
                        Success = true
                    };
                }
                else
                {
                    return new ActionResultBasic()
                    {
                        Success = false,
                        FailReason = failedReasonBuilder.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _controllersCollection.LoggingController.LogMessage(typeof(DoshiiController), DoshiiLogLevels.Error, string.Format(" There was an exception while attempting to sync Doshii members with the pos"), ex);
                return new ActionResultBasic()
                {
                    Success = false,
                    FailReason = DoshiiStrings.GetThereWasAnExceptionSeeLogForDetails("syncing apps")
                };
            }
        }
    }
}
