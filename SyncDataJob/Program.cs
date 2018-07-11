using Microsoft.SharePoint.Client;
using NPD.CommonDataContract;
using NPD.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncDataJob
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {


                DataTable dtProjectCategory = GetOracleData(ConfigurationManager.AppSettings["StoreProcedureName"], "MasterDataConnectionString");
                ////Add columns to DataTable.
                //dtProjectCategory.Columns.AddRange(new DataColumn[3] { new DataColumn("ProductCategory"), new DataColumn("ProductCategoryCode"), new DataColumn("BusinessUnit") });
                ////Set the Default Value.
                //dtProjectCategory.Rows.Add("FANS - Air Circulator", "AC", "CP");
                //dtProjectCategory.Rows.Add("Area Lighting", "AL", "LUM");
                //dtProjectCategory.Rows.Add("ABC", "AB", "LUM");

                DataTable dtProjectCategoryDetail = GetOracleData(ConfigurationManager.AppSettings["StoreProcedureName"], "MasterDataConnectionString");
                ////Add columns to DataTable.
                //dtProjectCategoryDetail.Columns.AddRange(new DataColumn[6] { new DataColumn("ProductCategory"), new DataColumn("ProductCategoryCode"), new DataColumn("Active"), new DataColumn("Phased"), new DataColumn("Sales"), new DataColumn("Average") });
                ////Set the Default Value.
                //dtProjectCategoryDetail.Rows.Add("FANS - Air Circulator", "AC", "15", "20", "5968624.44", "4968624.44");
                //dtProjectCategoryDetail.Rows.Add("ABC", "AB", "15", "20", "5968624.44", "4968624.44");

                string rootURL = BELDataAccessLayer.Instance.GetSiteURL(SiteURLs.ROOTSITEURL);
                using (ClientContext clientContext = BELDataAccessLayer.Instance.CreateClientContext(rootURL))
                {
                    Web web = BELDataAccessLayer.Instance.CreateWeb(clientContext);
                    SaveDataInProductCategoryList(clientContext, web, ListNames.PRODUCTCATEGORYMASTER, dtProjectCategory);
                    SaveDataInProductCategoryDetailList(clientContext, web, ListNames.PRODUCTCATEGORYDETAILMASTER, dtProjectCategoryDetail);
                }
                Logger.Info("Process Completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error While Execute Main Method");
                Console.Write(ex.StackTrace + "==>" + ex.Message);
                Logger.Error(ex);
            }
        }

        public static void SaveDataInProductCategoryList(ClientContext context, Web web, string listName, DataTable dataTable)
        {
            try
            {
                List productCategoryList = web.Lists.GetByTitle(listName);
                CamlQuery configCaml = new CamlQuery();
                ListItemCollection items = productCategoryList.GetItems(configCaml);
                context.Load(productCategoryList);

                context.Load(items, includes => includes.Include(i => i["ID"], i => i["ProductCategoryCode"], i => i["Title"], i => i["ProductCategory"], i => i["BusinessUnit"]));
                List businessUnit = web.Lists.GetByTitle(ListNames.BUSINESSUNITMASTER);

                ListItemCollection businessunititems = businessUnit.GetItems(configCaml);

                context.Load(businessunititems, includes => includes.Include(i => i["ID"], i => i["Code"]));
                context.ExecuteQuery();
                List<ListItem> lstProjectCategory = items.ToList();

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (items != null && items.Count > 0 && lstProjectCategory.Exists(x => Convert.ToString(x.FieldValues["ProductCategoryCode"]) == Convert.ToString(row["ProductCategoryCode"]) && Convert.ToString(((FieldLookupValue)x.FieldValues["BusinessUnit"]).LookupValue) == Convert.ToString(row["BusinessUnit"])))
                        {
                            try
                            {
                                ListItem currItem = lstProjectCategory.SingleOrDefault(x => Convert.ToString(x.FieldValues["ProductCategoryCode"]) == Convert.ToString(row["ProductCategoryCode"]) && Convert.ToString(((FieldLookupValue)x.FieldValues["BusinessUnit"]).LookupValue) == Convert.ToString(row["BusinessUnit"]));
                                currItem["ProductCategoryCode"] = Convert.ToString(row["ProductCategoryCode"]);
                                currItem["Title"] = Convert.ToString(row["ProductCategory"]);
                                currItem["ProductCategory"] = Convert.ToString(row["ProductCategory"]);
                                currItem["BusinessUnit"] = businessunititems.SingleOrDefault(x => Convert.ToString(x.FieldValues["Code"]) == Convert.ToString(row["BusinessUnit"])).FieldValues["ID"];
                                currItem.Update();
                                context.ExecuteQuery();
                            }
                            catch (Exception ex)
                            {


                            }

                        }
                        else
                        {
                            ListItemCreationInformation listCreationInformation = new ListItemCreationInformation();
                            ListItem oListItem = productCategoryList.AddItem(listCreationInformation);
                            oListItem["ProductCategoryCode"] = Convert.ToString(row["ProductCategoryCode"]);
                            oListItem["Title"] = Convert.ToString(row["ProductCategory"]);
                            oListItem["ProductCategory"] = Convert.ToString(row["ProductCategory"]);
                            oListItem["BusinessUnit"] = businessunititems.SingleOrDefault(x => Convert.ToString(x.FieldValues["Code"]) == Convert.ToString(row["BusinessUnit"])).FieldValues["ID"];
                            oListItem.Update();
                            context.ExecuteQuery();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error While Save Project Catgory List");
                Console.Write(ex.StackTrace + "==>" + ex.Message);
                Logger.Error(ex);
            }
            
        }

        public static void SaveDataInProductCategoryDetailList(ClientContext context, Web web, string listName, DataTable dataTable)
        {
            try
            {
                List productCategoryList = web.Lists.GetByTitle(ListNames.PRODUCTCATEGORYMASTER);
                List productCategoryDetailList = web.Lists.GetByTitle(listName);
                CamlQuery configCaml = new CamlQuery();
                ListItemCollection items = productCategoryList.GetItems(configCaml);
                ListItemCollection productCategoryDetailitems = productCategoryDetailList.GetItems(configCaml);
                context.Load(productCategoryList);
                context.Load(items);
                context.Load(productCategoryDetailList);
                //clientContext.Load(productCategoryDetailitems);
                context.Load(productCategoryDetailitems, includes => includes.Include(i => i["ID"], i => i["ProductCategory_x003a_ProductCat"], i => i["CurrentSKUs"], i => i["PhasedOutSKUs"], i => i["CurrentSales"], i => i["AverageSales"]));
                context.ExecuteQuery();

                List<ListItem> lstproductCategoryDetail = productCategoryDetailitems.ToList();

                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (productCategoryDetailitems != null && productCategoryDetailitems.Count > 0 && lstproductCategoryDetail.Exists(x => Convert.ToString(((FieldLookupValue)x.FieldValues["ProductCategory"]).LookupValue) == Convert.ToString(row["ProductCategoryCode"]) && Convert.ToString(((FieldLookupValue)x.FieldValues["ProductCategory_x003a_ProductCat"]).LookupValue) == Convert.ToString(row["ProductCategory"])))
                        {
                            try
                            {
                                ListItem currItem = lstproductCategoryDetail.SingleOrDefault(x => Convert.ToString(((FieldLookupValue)x.FieldValues["ProductCategory"]).LookupValue) == Convert.ToString(row["ProductCategoryCode"]) && Convert.ToString(((FieldLookupValue)x.FieldValues["ProductCategory_x003a_ProductCat"]).LookupValue) == Convert.ToString(row["ProductCategory"]));
                                currItem["CurrentSKUs"] = Convert.ToDouble(row["Active"]);
                                currItem["PhasedOutSKUs"] = Convert.ToDouble(row["Phased"]);
                                currItem["CurrentSales"] = Convert.ToDouble(row["Sales"]);
                                currItem["AverageSales"] = Convert.ToDouble(row["Average"]);

                                currItem.Update();
                                context.ExecuteQuery();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error While Execute Main Method");
                                Console.Write(ex.StackTrace + "==>" + ex.Message);
                                Logger.Error("Error While Execute Main Method");
                                Logger.Error(ex);

                            }

                        }
                        else
                        {
                            ListItemCreationInformation listCreationInformation = new ListItemCreationInformation();
                            ListItem oListItem = productCategoryDetailList.AddItem(listCreationInformation);
                            oListItem["CurrentSKUs"] = Convert.ToDouble(row["Active"]);
                            oListItem["PhasedOutSKUs"] = Convert.ToDouble(row["Phased"]);
                            oListItem["CurrentSales"] = Convert.ToDouble(row["Sales"]);
                            oListItem["AverageSales"] = Convert.ToDouble(row["Average"]);
                            oListItem["ProductCategory"] = Convert.ToDouble(row["Average"]);
                            oListItem["ProductCategory"] = items.SingleOrDefault(x => Convert.ToString(x.FieldValues["Title"]) == Convert.ToString(row["ProductCategory"]) && Convert.ToString(x.FieldValues["ProductCategoryCode"]) == Convert.ToString(row["ProductCategoryCode"])).FieldValues["ID"];
                            oListItem.Update();
                            context.ExecuteQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
                Console.WriteLine("Error While Save Project Catgory Detail List");
                Console.Write(ex.StackTrace + "==>" + ex.Message);
                Logger.Error(ex);
            }
            
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Data Table</returns>
        public static DataTable GetData(string storeProcedureName, string connectionName)
        {
            DataTable dt = new DataTable();
            try
            {
                DataSet ds = SqlHelper.ExecuteDataset(SqlHelper.GetConnectionString(connectionName), System.Data.CommandType.StoredProcedure, storeProcedureName);
                if (ds != null && ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "Table";
                    DataTable existingdt = ds.Tables[0];
                    foreach (DataColumn dc in existingdt.Columns)
                    {
                        dt.Columns.Add(dc.ColumnName);
                    }
                    foreach (DataRow existingdr in existingdt.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        foreach (DataColumn dc in existingdt.Columns)
                        {
                            dr[dc.ColumnName] = existingdr[dc.ColumnName];
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error while fetching Records from DB");
                Logger.Error(ex);
            }
            return dt;
        }

        public static DataTable GetOracleData(string storeProcedureName, string connectionName)
        {
            DataTable dt = new DataTable();
            try
            {
                OracleConnection oracleConnection = new OracleConnection(SqlHelper.GetConnectionString(connectionName));
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = oracleConnection;
                cmd.CommandText = storeProcedureName;
                cmd.CommandType = CommandType.StoredProcedure;


                OracleDataAdapter da = new OracleDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                
                if (ds != null && ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "Table";
                    DataTable existingdt = ds.Tables[0];
                    foreach (DataColumn dc in existingdt.Columns)
                    {
                        dt.Columns.Add(dc.ColumnName);
                    }
                    foreach (DataRow existingdr in existingdt.Rows)
                    {
                        DataRow dr = dt.NewRow();
                        foreach (DataColumn dc in existingdt.Columns)
                        {
                            dr[dc.ColumnName] = existingdr[dc.ColumnName];
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error while fetching Records from DB");
                Logger.Error(ex);
            }
            return dt;
        }
    }
}
