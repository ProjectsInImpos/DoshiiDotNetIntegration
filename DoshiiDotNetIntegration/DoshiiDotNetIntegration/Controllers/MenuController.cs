﻿using System;
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
                _controllersCollection.LoggingController.LogMessage(typeof(TransactionController), DoshiiLogLevels.Fatal, "Doshii: Initialization failed - httpComs cannot be null");
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
        internal virtual Menu UpdateMenu(Menu menu)
        {
            Menu returnedMenu = null;
            try
            {
                returnedMenu = _httpComs.PostMenu(menu);
            }
            catch (Exception ex)
            {
                return null;
            }
            if (returnedMenu != null)
            {
                return returnedMenu;
            }
            else
            {
                return null;
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
        internal virtual Surcount UpdateSurcount(Surcount surcount)
        {
            if (surcount.Id == null || string.IsNullOrEmpty(surcount.Id))
            {
                _controllersCollection.LoggingController.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Error, "Surcounts must have an Id to be created or updated on Doshii");
            }
            Surcount returnedSurcharge = null;
            try
            {
                returnedSurcharge = _httpComs.PutSurcount(surcount);
            }
            catch (Exception ex)
            {
                return null;
            }
            if (returnedSurcharge != null)
            {
                return returnedSurcharge;
            }
            else
            {
                return null;
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
        internal virtual Product UpdateProduct(Product product)
        {
            if (product.PosId == null || string.IsNullOrEmpty(product.PosId))
            {
                _controllersCollection.LoggingController.mLog.LogDoshiiMessage(this.GetType(), DoshiiLogLevels.Error, "Products must have an Id to be created or updated on Doshii");
            }
            Product returnedProduct = null;
            try
            {
                returnedProduct = _httpComs.PutProduct(product);
            }
            catch (Exception ex)
            {
                return null;
            }
            if (returnedProduct != null)
            {
                return returnedProduct;
            }
            else
            {
                return null;
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
        internal virtual SurcountActionResult DeleteSurcount(string posId)
        {
            var actionResult = new SurcountActionResult();
            try
            {
                actionResult = _httpComs.DeleteSurcount(posId);
            }
            catch (Exception ex)
            {
                actionResult.Success = false;
                actionResult.FailReason = DoshiiStrings.GetUnknownErrorString("delete surcount");
            }
            return actionResult;
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
        internal virtual ActionResultBasic DeleteProduct(string posId)
        {
            bool success;
            try
            {
                success = _httpComs.DeleteProduct(posId);
            }
            catch (Exception ex)
            {
                return false;
            }
            return success;
        }
        
    }
}
