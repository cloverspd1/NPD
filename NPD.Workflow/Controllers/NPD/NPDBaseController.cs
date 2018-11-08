namespace NPD.Workflow.Controllers
{
    using CommonDataContract;
    using BusinessLayer;
    using Common;
    using Models;
    using Models.NPD;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using NPD.NWorkflow.BusinessLayer;

    /// <summary>
    /// MVR Base Controller
    /// </summary>
    public class NPDBaseController : BaseController
    {
        public NPDContract GetNPDDetails(IDictionary<string, string> objDict)
        {
            NPDContract contract = NPDBusinessLayer.Instance.GetNPDDetails(objDict);
            return contract;
        }

        /// <summary>
        /// Saves the section.
        /// </summary>
        /// <typeparam name="T">dict parameter</typeparam>
        /// <param name="section">The section.</param>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>return status</returns>
        protected ActionStatus SaveSection(ISection section, Dictionary<string, string> objDict)
        {
            ActionStatus status = new ActionStatus();
           status = NPDBusinessLayer.Instance.SaveBySection(section, objDict);
            return status;
        }

        //public NPDAdminContract GetNPDAdminDetails(IDictionary<string, string> objDict)
        //{
        //    return NPDBusinessLayer.Instance.GetNPDAdminDetails(objDict);
           
        //}

        //public List<NPDDetails> RetrieveAllNPDNos(IDictionary<string, string> objDict)
        //{
        //    return NPDBusinessLayer.Instance.RetrieveAllNPDNos(objDict);            
        //}

        //public NPDDetails RetrieveNPDNoDetails(IDictionary<string, string> objDict)
        //{
        //    NPDDetails NPDdetail = NPDBusinessLayer.Instance.RetrieveNPDNoDetails(objDict);
        //    return NPDdetail;
        //}

        /// <summary>
        /// Saves the section.
        /// </summary>
        /// <typeparam name="T">dict parameter</typeparam>
        /// <param name="section">The section.</param>
        /// <param name="objDict">The object dictionary.</param>
        /// <returns>return status</returns>


        public NPDContract GetNPDAdminDetails(IDictionary<string, string> objDict)
        {
            NPDContract contract = NPDBusinessLayer.Instance.GetNPDAdminDetails(objDict);
            return contract;
        }

         protected ActionStatus SaveAdminDetailSection(ISection section, Dictionary<string, string> objDict)
        {
            ActionStatus status = new ActionStatus();
            status = NPDBusinessLayer.Instance.SaveAdminDataBySection(section, objDict);
            return status;
        }

       
    }
}