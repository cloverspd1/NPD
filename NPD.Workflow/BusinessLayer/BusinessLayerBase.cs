namespace NPD.Workflow.BusinessLayer
{
    using CommonDataContract;
    using DataAccessLayer;
    using Microsoft.SharePoint.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web;


    public  class BusinessLayerBase
    {
        /// <summary>
        /// The context
        /// </summary>
        public static ClientContext context = null;

        /// <summary>
        /// The web
        /// </summary>
        public static Web web = null;

        protected BusinessLayerBase()
        {
            try
            {
                string siteURL = BELDataAccessLayer.Instance.GetSiteURL(SiteURLs.NPDSITEURL);
                if (!string.IsNullOrEmpty(siteURL))
                {
                    context = BELDataAccessLayer.Instance.CreateClientContext(siteURL);
                    web = BELDataAccessLayer.Instance.CreateWeb(context);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}