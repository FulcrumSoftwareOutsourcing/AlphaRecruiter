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

using System.Collections.Generic;

using Framework.Db;
using Framework.Entity;
using Framework.Metadata;
using Framework.Remote.Mobile;
using Framework.Utils;

namespace Framework.Remote
{
  /// <summary>
  /// Class incapsulating edit operations.
  /// </summary>
  public class CxEditController
  {
    private CxEntityUsageMetadata m_EntityUsage;
    //----------------------------------------------------------------------------
    public CxEditController(CxEntityUsageMetadata entityUsage)
    {
      m_EntityUsage = entityUsage;
    }

    /// <summary>
    /// Processes dependent fields of the given entity.
    /// Calculates local expressions, row source filters, read-only and visibility conditions.
    /// </summary>
    /// <param name="exprResult">CxExpressionResult to process</param>
    /// <param name="changedAttr">changed attribute</param>
    /// <param name="changedValue">changed value</param>
    virtual public void ProcessDependentFields(
      CxExpressionResult exprResult,
      CxAttributeMetadata changedAttr,
      object changedValue)
    {
      //CxBaseEntity entity_ = exprResult.ActualEntity;
      if (changedAttr != null)
      {
        //entity[changedAttr.Id] = changedValue;
        Dictionary<CxAttributeMetadata, bool> processedMap = new Dictionary<CxAttributeMetadata, bool>();
        ProcessDependentFields(exprResult, changedAttr, processedMap);
        // ReplaceFormControls(entity);
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Processes dependent fields of the given entity.
    /// Calculates local expressions, row source filters, read-only and visibility conditions.
    /// </summary>
    /// <param name="exprResult">CxExpressionResult to process</param>
    /// <param name="changedAttr">changed attribute</param>
    /// <param name="processedMap">map of the processed attributes</param>
    protected void ProcessDependentFields(
      CxExpressionResult exprResult,
      CxAttributeMetadata changedAttr,
      Dictionary<CxAttributeMetadata, bool> processedMap)
    {
      CxBaseEntity entity = exprResult.ActualEntity;
      IList<CxAttributeMetadata> dependentAttrs = entity.Metadata.GetDependentAttributes(changedAttr);
      if (dependentAttrs != null)
      {
        foreach (CxAttributeMetadata attribute in dependentAttrs)
        {
          if (!processedMap.ContainsKey(attribute))
          {
            processedMap.Add(attribute, true);
            ProcessDependentField(exprResult, attribute, processedMap);
          }
        }
      }

      
      IList<CxAttributeMetadata> dependentStateAttrs = entity.Metadata.GetDependentStateAttributes(changedAttr);
      if (dependentStateAttrs != null)
      {
        foreach (CxAttributeMetadata attribute in dependentStateAttrs)
        {
          ProcessDependentFieldState(exprResult, attribute);
        }
      }

    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Processes dependent field of the given entity.
    /// Calculates local expressions, row source filters, read-only and visibility conditions.
    /// </summary>
    /// <param name="exprResult">CxExpressionResult to process</param>
    /// <param name="dependentAttr">dependent attribute</param>
    /// <param name="processedMap">map of the processed attributes</param>
    protected void ProcessDependentField(
      CxExpressionResult exprResult,
      CxAttributeMetadata dependentAttr,
      Dictionary<CxAttributeMetadata, bool> processedMap)
    {
      CxBaseEntity entity = exprResult.ActualEntity;
      object oldValue = entity[dependentAttr.Id];
      object newValue = oldValue;

      // Process local expression
      if (CxUtils.NotEmpty(dependentAttr.LocalExpression))
      {
        using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
        {
          entity.SetCalculatedValue(connection, dependentAttr, true);
        }
        newValue = entity[dependentAttr.Id];
      }

      // Process row source filter
      if (dependentAttr.RowSource != null && CxUtils.NotEmpty(dependentAttr.RowSourceFilter))
      {
        using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
        {         
          RefreshDynamicLookup(exprResult, dependentAttr, true);
          string newText;
          dependentAttr.RowSource.GetDefaultValue(
            connection,
            dependentAttr.RowSourceFilter,
            entity,
            GetIsMandatory(dependentAttr, entity),
            dependentAttr.IsMultiValueLookup,
            newValue,
            out newValue,
            out newText);        

          // If value changed, process further dependencies
          if (CxUtils.ToString(oldValue) != CxUtils.ToString(newValue))
          {        
            entity[dependentAttr.Id] = newValue;        
            ProcessDependentFields(exprResult, dependentAttr, processedMap);
          }
        }
      }
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Processes dependent state of the field of the given entity.
    /// </summary>
    /// <param name="exprResult">CxExpressionResult to process</param>
    /// <param name="dependentAttr">dependent attribute</param>
    protected void ProcessDependentFieldState(
      CxExpressionResult exprResult,
      CxAttributeMetadata dependentAttr)
    {
      // Process read-only condition and visibility condition for form only.
      // For a grid view these conditions are calculated before show editor.
      //  CxBaseEntity entity = exprResult.ActualEntity;
      //  if (IsForm && m_Form != null)
      //  {
      //       if (CxUtils.NotEmpty(dependentAttr.ReadOnlyCondition) ||
      //         CxUtils.NotEmpty(dependentAttr.VisibilityCondition))
      //     {
      //    m_Form.UpdateControlState(entity.Metadata, dependentAttr);
      //   }
      // }
    }
    
    
    //-------------------------------------------------------------------------
    /// <summary>
    /// Returns true if given attribute should be mandatory in context of the given entity.
    /// </summary>
    /// <param name="attribute">attribute metadata</param>
    /// <param name="entity">entity or null if entity is not present</param>
    public static bool GetIsMandatory(CxAttributeMetadata attribute, CxBaseEntity entity)
    {
      return attribute != null ?
        (entity != null ? entity.IsMandatory(attribute.Id) : !attribute.Nullable) : false;
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Refreshes dynamic lookup data source
    /// </summary>
    /// <param name="exprResult">CxExpressionResult to process</param>
    /// <param name="attribute">attribute metadata</param>
    /// <param name="overwrite">true to overwrite existing datasource</param>
    protected void RefreshDynamicLookup(
      CxExpressionResult exprResult,
      CxAttributeMetadata attribute,
      bool overwrite)
    {
      CxRowSourceMetadata rs = attribute.RowSource;
      IList<CxComboItem> items;
      using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
      {
        items = rs.GetList(
          null,
          connection,
          attribute.GetRowSourceFilter(exprResult.ActualEntity),
          exprResult.ActualEntity,
          !GetIsMandatory(attribute, exprResult.ActualEntity) || false);
      }

      CxClientRowSource rowSource = new CxClientRowSource(items, exprResult.ActualEntity, attribute);
      if (rowSource.IsFilteredRowSource)
      {
        exprResult.FilteredRowSources.Add(rowSource);
      }
      else
      {
        exprResult.UnfilteredRowSources.Add(rowSource.RowSourceId.ToUpper(), rowSource);
      }

    }
    //-------------------------------------------------------------------------
    /// <summary>
    ///  Gets dynamic lookup data source
    /// </summary>
    /// <param name="attribute">attribute metadata</param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public CxClientRowSource GetDynamicRowSource(
      CxAttributeMetadata attribute,
      CxBaseEntity entity)
    {

      CxRowSourceMetadata rs = attribute.RowSource;
      IList<CxComboItem> items;
      CxExpressionResult exprResult = new CxExpressionResult { ActualEntity = entity };
      using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
      {
        items = rs.GetList(
          null,
          connection,
          attribute.GetRowSourceFilter(exprResult.ActualEntity),
          exprResult.ActualEntity,
          !GetIsMandatory(attribute, exprResult.ActualEntity) || false);
      }
      return new CxClientRowSource(items, exprResult.ActualEntity, attribute);
    }
    //----------------------------------------------------------------------------

    /// <summary>
    /// Returns true if control corresponding to the given attribute should be
    /// visible in context of the given entity.
    /// </summary>
    /// <param name="attribute">attribute metadata</param>
    /// <param name="entity">entity instance</param>
    virtual public bool GetIsVisible(
      CxAttributeMetadata attribute,
      CxBaseEntity entity)
    {
      bool isVisible = m_EntityUsage.GetIsAttributeEditable(attribute);
      if (isVisible)
      {
        if (entity != null && CxUtils.NotEmpty(attribute.VisibilityCondition))
        {
          using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
          {
            isVisible = entity.CalculateBoolExpression(connection, attribute.VisibilityCondition);
          }
        }
        if (isVisible)
        {
          using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
          {
            isVisible = attribute.GetIsVisible(m_EntityUsage, connection, entity);
          }
        }
      }
      return isVisible;
    }

    //----------------------------------------------------------------------------
    /// <summary>
    /// Returns true if control corresponding to the given attribute should be
    /// readonly in context of the given entity.
    /// </summary>
    /// <param name="attribute">attribute metadata</param>
    /// <param name="entity">entity instance</param>
    virtual public bool GetIsReadOnly(
      CxAttributeMetadata attribute,
      CxBaseEntity entity)
    {
      bool isReadOnly = (attribute.ReadOnly) ||
                        (m_EntityUsage.ReadOnlyAttributes);

      // Check dynamic read-only state of the column.
      if (!isReadOnly && entity != null && CxUtils.NotEmpty(attribute.ReadOnlyCondition))
      {
        using (CxDbConnection connection = CxDbConnections.CreateEntityConnection())
        {
          isReadOnly = entity.CalculateBoolExpression(connection, attribute.ReadOnlyCondition);
        }
      }
      return isReadOnly;
    }
  }
}
