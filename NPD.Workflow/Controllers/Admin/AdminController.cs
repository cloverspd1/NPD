﻿namespace NPD.Workflow.Controllers
{
    using NPD.CommonDataContract;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.SharePoint.Client;
    using NPD.NWorkflow;
    using NPD.Workflow.BusinessLayer;

    /// <summary>
    /// Admin
    /// </summary>
    public partial class AdminController : BaseController
    {
        #region "Cache Clear"
        /// <summary>
        /// Generates the RMLSMW.
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns>Action Result</returns>
        public ActionResult ClearCache(string ids)
        {
            ActionStatus status = new ActionStatus();
            if (!string.IsNullOrEmpty(ids))
            {

                List<string> keys = ids.Split(',').ToList();
                CommonBusinessLayer.Instance.RemoveCacheKeys(keys);
                status.IsSucceed = true;
                status.Messages.Add("Selected Cache key(s) has been Cleared.");

            }
            return this.Json(status);
        }

        /// <summary>
        /// Gets the LSMW list.
        /// </summary>
        /// <returns>return list of LSMW</returns>
        public ActionResult CacheList()
        {
            List<string> strList = CommonBusinessLayer.Instance.GetCacheKeys();
            strList = strList.OrderBy(i => i).ToList();
            return this.View("CacheList", strList);
        }


        #endregion
    }
}