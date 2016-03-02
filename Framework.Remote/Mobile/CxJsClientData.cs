using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Db;
using Framework.Entity;
using Framework.Metadata;
using System.Configuration;

namespace Framework.Remote.Mobile
{
    public class CxJsClientData
    {
        public CxJsClientData()
        {
        }

        public CxJsClientData(Metadata.CxEntityUsageMetadata entityUsage,
            IEnumerable<CxBaseEntity> entities,
            CxDbConnection conn)
        {
            Rows = new List<List<CxDataItem>>();
            SortDescriptions = new List<CxSortDescriptionJs>();
            UnfilteredRowSources = new Dictionary<string, CxClientRowSource>();
            FilteredRowSources = new Dictionary<string, CxClientRowSource>();
            ApplicationValues = new Dictionary<string, object>();
            PksIndexesInSet = new Dictionary<string, int>();

            CreateData(entityUsage, entities, conn);
        }

        public List<string> AttrsInSet { get; private set; }
        public Dictionary<string, int> PksIndexesInSet { get; private set; }
        public List< List< CxDataItem> > Rows { get; private set; }

        public string EntityUsageId {get; set;}

        public int TotalDataRecordAmount {get; set;}

        public List<CxSortDescriptionJs> SortDescriptions { get; set; }

        public Dictionary<string, CxClientRowSource> UnfilteredRowSources { get; set; }

        public Dictionary<string, CxClientRowSource> FilteredRowSources { get; set; }

        public bool IsNewEntity { get; set; }

        public Dictionary<string, object> ApplicationValues { get; private set; }

        public CxClientEntityMarks EntityMarks { get; set; }



        private void CreateData(Metadata.CxEntityUsageMetadata entityUsage,
            IEnumerable<CxBaseEntity> entities,
            CxDbConnection conn)
        {
            AttrsInSet = new List<string>();
            Dictionary<string, object> cols = new Dictionary<string, object>();

            foreach (CxBaseEntity entity in entities)
            {
                List<CxDataItem> row = new List<CxDataItem>();
                int index = 0;
                foreach (Metadata.CxAttributeMetadata attribute in entityUsage.Attributes)
                {
                    if (attribute.PrimaryKey && PksIndexesInSet.ContainsKey(attribute.Id) == false)
                        PksIndexesInSet.Add(attribute.Id, index);

                    index++;

                    if (!cols.ContainsKey(attribute.Id))
                        cols.Add(attribute.Id, null);

                    CxDataItem dataItem = new CxDataItem();
                    dataItem.Value = GetJsObject(entity[attribute.Id], attribute);
                    dataItem.Readonly = attribute.ReadOnly;
                    dataItem.Visible = attribute.Visible;

                    if (!string.IsNullOrEmpty(attribute.ReadOnlyCondition))
                    {
                        dataItem.Readonly = entity.CalculateBoolExpression(conn, attribute.ReadOnlyCondition);
                    }
                    if (!string.IsNullOrEmpty(attribute.VisibilityCondition))
                    {
                        dataItem.Visible = entity.CalculateBoolExpression(conn, attribute.VisibilityCondition);
                    }

                    if (attribute.PrimaryKey)
                    {
                        //calculate command disable conditions

                        IEnumerable<CxCommandMetadata> cmdWithConditions =
                          entityUsage.Commands.Where(c => c.DisableConditions.Count > 0);
                        if (cmdWithConditions.Count() > 0)
                        {
                            dataItem.DisabledCommandIds = new Dictionary<string, string>();
                        }

                        foreach (CxCommandMetadata command in entityUsage.Commands)
                        {
                            if (command.DisableConditions.Count > 0)
                            {
                                bool hasDisabled = false;
                                foreach (CxErrorConditionMetadata condition in command.DisableConditions)
                                {
                                    bool result = entity.CalculateBoolExpression(conn, condition.Expression);
                                    if (result)
                                    {
                                        if (!dataItem.DisabledCommandIds.ContainsKey(command.Id))
                                        {
                                            dataItem.DisabledCommandIds.Add(command.Id, condition.ErrorText);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    row.Add(dataItem);

                }

                Rows.Add(row);

            }
            AttrsInSet.AddRange(cols.Keys);
        }

        private static string ServerDateFormat = ConfigurationManager.AppSettings["ServerDateFormat"];
        private static string ServerDateTimeFormat = ConfigurationManager.AppSettings["ServerDateTimeFormat"];
        

        private static object GetJsObject(object val, CxAttributeMetadata attr)
        {
            if (attr.Type == "datetime")
            {
                //var jsDate = new
                //{ 
                //    Str = val != null ? ((DateTime)val).ToString(ServerDateTimeFormat) : val,
                //    Value = val != null ? ((DateTime)val).Ticks : val,
                //};
                //return jsDate;
                return val != null ? ((DateTime)val).ToString(ServerDateTimeFormat) : val;
            }
            if (attr.Type == "date")
            {
                //var jsDate = new
                //{ 
                //    Str = val != null ? ((DateTime)val).ToString(ServerDateFormat) : val,
                //    Vale = val != null ? ((DateTime)val).Ticks : val,
                //};
                //return jsDate;
                return val != null ? ((DateTime)val).ToString(ServerDateFormat) : val;
            }

            return val;
        }

        public static void FixJsObjects(IDictionary<string, object> values, CxEntityUsageMetadata entityUsage)
        {
            if (values == null)
                return;


            foreach (var attr in entityUsage.Attributes)
            {
                if ((attr.Type == "datetime" ) && values.ContainsKey(attr.Id))
                {
                    string jsVal = Convert.ToString(values[attr.Id]);
                    if (string.IsNullOrWhiteSpace(jsVal) == false)
                    {
                        values[attr.Id] = DateTime.ParseExact(jsVal, ServerDateTimeFormat, null);
                    }
                    //if (jsVal is long)
                      //  values[attr.Id] = new DateTime((long)values[attr.Id]);
                }
                if ((attr.Type == "date") && values.ContainsKey(attr.Id))
                {
                    string jsVal = Convert.ToString(values[attr.Id]);
                    if (string.IsNullOrWhiteSpace(jsVal) == false)
                    {
                        values[attr.Id] = DateTime.ParseExact(jsVal, ServerDateFormat, null);
                    }
                    //if (jsVal is long)
                    //  values[attr.Id] = new DateTime((long)values[attr.Id]);
                }
            }
        }

    }
}
