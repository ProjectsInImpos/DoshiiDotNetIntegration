﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoshiiDotNetIntegration.Interfaces
{
    public interface iDoshiiProducts
    {
        /// <summary>
        /// this method should be used to post new products to doshii
        /// when implementing this method doshiiLodic.AddNewProducts MUST be called
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        bool AddNewProducts(DoshiiOperationLogic doshiiLogic, List<Models.Product> productList);

        /// <summary>
        /// this method should be used to update a product on doshii
        /// when implementing this method doshiiLodic.UpdateProcuct MUST be called
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <param name="productToUpdate"></param>
        /// <returns></returns>
        bool UpdateProduct(DoshiiOperationLogic doshiiLogic, Models.Product productToUpdate);

        /// <summary>
        /// this method should be used to delete products from doshii
        /// when implementing this method doshiiLodic.DeleteProducts MUST be called
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <param name="productIdList"></param>
        /// <returns></returns>
        bool DeleteProducts(DoshiiOperationLogic doshiiLogic, List<string> productIdList);

        /// <summary>
        /// this method should be used to delete all products from doshii
        /// when implementing this method doshiiLodic.DeleteAllProducts MUST be called
        /// </summary>
        /// <param name="doshiiLogic"></param>
        /// <returns></returns>
        bool DeleteAllProducts(DoshiiOperationLogic doshiiLogic);

        /// <summary>
        /// this method should be used to retreive all products from doshii
        /// when implementing this method doshiiLodic.GetAllProducts should be returned
        /// </summary>
        /// <returns></returns>
        List<Models.Product> GetAllProducts(DoshiiOperationLogic doshiiLogic);
    }
}