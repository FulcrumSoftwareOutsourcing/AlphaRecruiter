using System;
using System.Collections.Generic;
using System.Text;
using Framework.Utils;
using Framework.Metadata;

namespace Framework.Entity
{
  public class CxLanguageEntity : CxBaseEntity
  {
    //----------------------------------------------------------------------------
    public CxLanguageEntity(CxEntityUsageMetadata entityUsage)
      : base(entityUsage)
    {
    }
    //----------------------------------------------------------------------------
    /// <summary>
    /// Validates entity.
    /// </summary>
    public override void Validate()
    {
      if (CxBool.Parse(this["IsDefault"], false) && !CxBool.Parse(this["IsUserVisible"], false))
        throw new ExValidationException(
          Metadata.Holder.GetErr("The Default language cannot be made invisible. Please either make the language non-Default or make it Visible in order to proceed."));
      base.Validate();
    }
    //----------------------------------------------------------------------------
  }
}
