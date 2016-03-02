using System;
using System.Text;
using Framework.Metadata.PlaceholderManager;

namespace Framework.Metadata
{
  public abstract class CxPlaceholderManagerBase
  {
    //-------------------------------------------------------------------------
    protected CxPlaceholderManagerBase()
    {
    }
    //-------------------------------------------------------------------------
    protected CxPlaceholderPosition GetNextPlaceholder(string str, CxPlaceholderPosition currentPosition)
    {
      CxPlaceholderPosition nextPosition = new CxPlaceholderPosition();
      int startIndex = 0;
      if (currentPosition != null)
        startIndex = currentPosition.StartIndex + currentPosition.Length;
      nextPosition.StartIndex = str.IndexOf('%', startIndex);
      if (nextPosition.StartIndex > -1)
      {
        int endIndex = str.IndexOf('%', nextPosition.StartIndex + 1);
        if (endIndex > -1)
        {
          nextPosition.Length = endIndex - nextPosition.StartIndex;
          return nextPosition;
        }
      }
      return null;
    }
    //-------------------------------------------------------------------------
    /// <summary>
    /// Processes the given placeholder. Returns null if no procession was done - placeholder was not recognized.
    /// </summary>
    /// <param name="placeholder">placeholder</param>
    /// <param name="languageCd">language code to export (current if empty)</param>
    /// <returns>value to replace placeholder with; null if the placeholder was invalid</returns>
    protected virtual string ProcessPlaceholder(string placeholder, string languageCd)
    {
      // In the default case we return the null value to indicate that no procession was done.
      return null;
    }
    //-------------------------------------------------------------------------
    public virtual string ReplacePlaceholders(string input)
    {
      return ReplacePlaceholders(input, null);
    }
    //-------------------------------------------------------------------------
    public virtual string ReplacePlaceholders(string input, string languageCd)
    {
      StringBuilder output = new StringBuilder();

      CxPlaceholderPosition currentPosition = null;
      CxPlaceholderPosition nextPosition = GetNextPlaceholder(input, null);

      while (nextPosition != null)
      {
        // Here we append the non-changable piece of the input text.
        if (currentPosition == null)
        {
          output.Append(input.Substring(0, nextPosition.StartIndex));
        }
        else
        {
          int startIndex = currentPosition.StartIndex + currentPosition.Length;
          int length = nextPosition.StartIndex - startIndex;
          output.Append(input.Substring(startIndex, length));
        }

        // Set the current position
        currentPosition = nextPosition;

        string placeholder = input.Substring(currentPosition.StartIndex + 1, currentPosition.Length - 1);
        string processedPlaceholder = ProcessPlaceholder(placeholder, languageCd);
        if (processedPlaceholder != null)
        {
          output.Append(processedPlaceholder);
          // Here we ensure that the trailing "%" won't be considered anymore.
          currentPosition.Length += 1;
        }
        else
          output.Append(input.Substring(currentPosition.StartIndex, currentPosition.Length));

        nextPosition = GetNextPlaceholder(input, currentPosition);
      }
      
      // Here we append all the appendix (ending) of the string.
      if (currentPosition != null)
        output.Append(input.Substring(currentPosition.StartIndex + currentPosition.Length));
      else
        output.Append(input);

      return output.ToString();
    }
    //-------------------------------------------------------------------------
  }
}
