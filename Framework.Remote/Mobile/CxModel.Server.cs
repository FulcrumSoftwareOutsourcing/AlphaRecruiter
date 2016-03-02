using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Db;
using Framework.Entity;
using Framework.Metadata;

namespace Framework.Remote.Mobile
{
  public partial class CxModel
  {

   

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

      Data = new List<CxDataItem>(entityUsage.Attributes.Count * entities.Count());

      int index = 0;
      foreach (CxBaseEntity entity in entities)
      {

        foreach (Metadata.CxAttributeMetadata attribute in entityUsage.Attributes)
        {
          Data.Add(new CxDataItem());
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
            if (cmdWithConditions.Count() > 0)
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
  }
}
