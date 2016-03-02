using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Entity
{
  [Flags]
  public enum NxAutomaticallyApplyChangesToDatabaseWhenMovingOffEntity
  {
    DoNothing = 0,
    Automatically = 1,
    PromptUser = 2
  }
}
