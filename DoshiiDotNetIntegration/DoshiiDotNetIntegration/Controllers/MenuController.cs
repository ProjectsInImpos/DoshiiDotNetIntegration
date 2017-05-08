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
    /// <summary>
    /// This class is used internally to mange the bl for keeping the menu upto date between the pos and Doshii
    /// </summary>
    internal class MenuController
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
        internal MenuController(Models.ControllersCollection controllerCollection, HttpController httpComs)
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
        
        /// <summary>
        /// updates the entire menu on Doshii, This call will overwrite any menu that currently exists on Doshii with the menu you have provided in the menu param. 
        /// </summary>
        /// <param name="menu">
        /// The full venue menu to overwrite the current menu in Doshii
        /// </param>
        /// <returns>
        /// The Doshii menu is successful and null if not successful. 
        /// </returns>
        internal virtual ObjectActionResult<Menu> UpdateMenu(Menu menu)
        {
            try
            {
                return _httpComs.PostMenu(menu);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// updates a surcount on Doshii
        /// </summary>
        /// <param name="surcount">
        /// the surcount that should be updated. 
        /// </param>
        /// <returns>
        /// The updated surcount. 
        /// </returns>
        internal virtual ObjectActionResult<Surcount> UpdateSurcount(Surcount surcount)
        {
            if (surcount.Id == null || string.IsNullOrEmpty(surcount.Id))
            {
                _controllersCollection.LoggingController.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Error, "Surcounts must have an Id to be created or updated on Doshii");
            }
            Surcount returnedSurcharge = null;
            try
            {
                return _httpComs.PutSurcount(surcount);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Updates a product on Doshii
        /// </summary>
        /// <param name="product">
        /// The product to be updated. 
        /// </param>
        /// <returns>
        /// The updated product. 
        /// </returns>
        internal virtual ObjectActionResult<Product> UpdateProduct(Product product)
        {
            if (product.PosId == null || string.IsNullOrEmpty(product.PosId))
            {
                _controllersCollection.LoggingController.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Error, "Products must have an Id to be created or updated on Doshii");
            }
            Product returnedProduct = null;
            try
            {
                return _httpComs.PutProduct(product);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// deletes a surcount on Doshii
        /// </summary>
        /// <param name="posId">
        /// the posId of the surcount to update. 
        /// </param>
        /// <returns>
        /// True if the surcount on Doshii was updated. 
        /// False if the surcount on doshii was not updated. 
        /// </returns>
        internal virtual ActionResultBasic DeleteSurcount(string surcountPosId)
        {
            if (string.IsNullOrEmpty(surcountPosId))
            {
                _controllersCollection.LoggingController.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Warning, DoshiiStrings.GetAttemptingActionWithEmptyId("delete a surcount", "surcount"));
                return new ActionResultBasic()
                {
                    Success = false,
                    FailReason = "surcountId was empty"
                };
            }
            else
            {
                try
                {
                    return _httpComs.DeleteSurcount(surcountPosId);
                }
                catch (Exceptions.RestfulApiErrorResponseException rex)
                {
                    throw rex;
                }
            }
         }

        /// <summary>
        /// Deletes a product on Doshii
        /// </summary>
        /// <param name="posId">
        /// the posId of the product to delete. 
        /// </param>
        /// <returns>
        /// True if the product was deleted 
        /// False if the product was not deleted. 
        /// </returns>
        internal virtual ActionResultBasic DeleteProduct(string productPosId)
        {
            if (string.IsNullOrEmpty(productPosId))
            {
                _controllersCollection.LoggingController.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Warning, DoshiiStrings.GetAttemptingActionWithEmptyId("delete a product", "product"));
                return new ActionResultBasic()
                {
                    Success = false,
                    FailReason = "productId was empty"
                };
            }
            else
            {
                try
                {
                    return _httpComs.DeleteSurcount(productPosId);
                }
                catch (Exceptions.RestfulApiErrorResponseException rex)
                {
                    throw rex;
                }
            }
        }
        
    }
}
