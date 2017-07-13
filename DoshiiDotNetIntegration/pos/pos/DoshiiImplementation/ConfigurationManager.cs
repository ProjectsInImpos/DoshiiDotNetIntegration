using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoshiiDotNetIntegration;
using DoshiiDotNetIntegration.Interfaces;
using pos.DoshiiImplementation;
using pos.Helpers;

namespace pos
{
    public class ConfigurationManager : IConfigurationManager
    {
        public DisplayHelper DHelper;
        public PosDoshiiController DControler;
        public ConfigurationManager(DisplayHelper dHelper, PosDoshiiController doshiiController)
        {
            DHelper = dHelper;
            DControler = doshiiController;
        }

        public string GetSocketUrlFromPos()
        {
            return LiveData.PosSettings.socketUrl;
        }

        public string GetBaseUrlFromPos()
        {
            return LiveData.PosSettings.BaseUrl;
        }

        public string GetLocationTokenFromPos()
        {
            return LiveData.PosSettings.locationToken;
        }

        public string GetSecretKeyFromPos()
        {
            return LiveData.PosSettings.secretKey;
        }

        public int GetSocketTimeOutFromPos()
        {
            return LiveData.PosSettings.socketTimeOutSec;
        }

        public string GetVendorFromPos()
        {
            return LiveData.PosSettings.doshiiVendor;
        }

        public IOrderingManager GetOrderingManagerFromPos()
        {
            return new OrderingManager(DControler);
        }

        public ITransactionManager GetTransactionManagerFromPos()
        {
            return new TransactionManager();
        }

        public IRewardManager GetRewardManagerFromPos()
        {
            if (LiveData.PosSettings.UseMembership)
            {
                return new RewardManager();
            }
            else
            {
                return null;
            }
        }

        public IReservationManager GetReservationManagerFromPos()
        {
            if (LiveData.PosSettings.UseReservations)
            {
                return new ReservationManager();
            }
            else
            {
                return null;
            }
        }

        public ILoggingManager GetLoggingManagerFromPos()
        {
            return new LoggingManager(DHelper);
        }

        public IAppManager GetAppManagerFromPos()
        {
            if (LiveData.PosSettings.UseApps)
            {
                return new AppManager();
            }
            else
            {
                return null;
            }
        }
    }
}
