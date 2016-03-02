/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Framework.Db;
using Framework.Entity;
using Framework.Metadata;

namespace Framework.Remote
{
  [DataContract]
  public sealed class CxModel : IxErrorContainer
  {

    [DataMember]
    public Guid Marker;

    [DataMember]
    public string EntityUsageId;

    [DataMember]
    public CxDataItem[] Data;

    [DataMember]
    public int TotalDataRecordAmount;

    [DataMember]
    public CxSortDescription[] SortDescriptions = new CxSortDescription[] { };

    [DataMember]
    public Dictionary<string, CxClientRowSource> UnfilteredRowSources = new Dictionary<string, CxClientRowSource>();

    [DataMember]
    public List<CxClientRowSource> FilteredRowSources = new List<CxClientRowSource>();

    [DataMember]
    public CxExceptionDetails Error { get; internal set; }

    [DataMember]
    public bool IsNewEntity { get; internal set; }

    [DataMember]
    public Dictionary<string, object> ApplicationValues = new Dictionary<string, object>();

    [DataMember]
    public CxClientEntityMarks EntityMarks { get; set; }

    //----------------------------------------------------------------------------
    internal CxModel(Guid marker)
    {
      Marker = marker;
      EntityUsageId = string.Empty;
      Data = null;
    }
    //----------------------------------------------------------------------------
    public void SetData(
      Metadata.CxEntityUsageMetadata entityUsage,
      IEnumerable<CxBaseEntity> entities,
      CxDbConnection conn)
    {
     
      Data = new CxDataItem[entityUsage.Attributes.Count * entities.Count()];

      int index = 0;
      foreach (CxBaseEntity entity in entities)
      {
        
        foreach (Metadata.CxAttributeMetadata attribute in entityUsage.Attributes)
        {
          Data[index] = new CxDataItem();
          Data[index].Value = entity[attribute.Id];
          Data[index].Readonly = attribute.ReadOnly;
          Data[index].Visible = attribute.Visible;

          if (!string.IsNullOrEmpty(attribute.ReadOnlyCondition))
          {
            Data[index].Readonly = entity.CalculateBoolExpression(conn, attribute.ReadOnlyCondition);
          }
          if (!string.IsNullOrEmpty(attribute.VisibilityCondition))
          {
            Data[index].Visible = entity.CalculateBoolExpression(conn, attribute.VisibilityCondition);
          }


          if (attribute.PrimaryKey)
          {
            //calculate command disable conditions

            IEnumerable<CxCommandMetadata> cmdWithConditions =
              entityUsage.Commands.Where(c => c.DisableConditions.Count > 0);
            if(cmdWithConditions.Count() > 0)
            {
              Data[index].DisabledCommandIds = new Dictionary<string, string>();
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
                    if (!Data[index].DisabledCommandIds.ContainsKey(command.Id))
                    {
                      Data[index].DisabledCommandIds.Add(command.Id, condition.ErrorText);
                    }
                  }
                }
              }
            }
          }

          if (Data[index].Value is DBNull)
          {
            Data[index].Value = null;
          }

          index++;
        }

      }
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Default ctor.
    /// </summary>
    public CxModel()
    {

    }
    //-------------------------------------------------------------------------
  }
}
