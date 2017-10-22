using System.ComponentModel;
using System.Threading;
using DoshiiDotNetIntegration.Interfaces;

namespace pos.DoshiiImplementation
{
    public class CancellationManager : ICancellationProvider
    {
        private readonly CancellationToken _token;


        public bool IsCancellationRequested
        {
            get { return _token.IsCancellationRequested; }
        }

       

        public CancellationManager(CancellationToken token)
        {
            _token = token;
        }
    }
}
