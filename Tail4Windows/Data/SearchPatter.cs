using System;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// Search pattern object 
  /// </summary>
  public class SearchPatter
  {
    /// <summary>
    /// Pattern
    /// </summary>
    public Pattern Pattern
    {
      get;
      set;
    }

    /// <summary>
    /// Pattern parts
    /// </summary>
    public Part PatternPart
    {
      get;
      set;
    }

    /// <summary>
    /// Clears all resources
    /// </summary>
    public void Clear()
    {
      Pattern = null;
      PatternPart = null;
    }

    /// <summary>
    /// Create copy of object
    /// </summary>
    /// <returns>A clone of object</returns>
    public SearchPatter Clone()
    {
      return MemberwiseClone() as SearchPatter;
    }

    /// <summary>
    /// Save data to memento
    /// </summary>
    /// <returns>Copy of SearchPattern</returns>
    public MementoSearchPattern SaveToMemento()
    {
      return new MementoSearchPattern(this);
    }

    /// <summary>
    /// Roll object back to the state of the provided memento
    /// </summary>
    /// <param name="memento">The memento to roll back to</param>
    /// <exception cref="ArgumentException">If memento object is no an MementoSearchPattern object</exception>
    public void RestoreFromMemento(MementoSearchPattern memento)
    {
      MementoSearchPattern mementoData = memento as MementoSearchPattern;
      Arg.NotNull(mementoData, "Argument is not a MementoSearchPattern");

      Pattern = mementoData.Pattern;
      PatternPart = mementoData.PatternPart;
    }

    /// <summary>
    /// Equals two objects
    /// </summary>
    /// <param name="obj">Reference of FileManagerData</param>
    /// <returns>If equal true otherwise false</returns>
    public bool EqualsProperties(object obj)
    {
      MementoSearchPattern other = obj as MementoSearchPattern;

      if(other == null)
        return false;

      bool equal = true;

      equal &= Equals(other.Pattern, Pattern);
      equal &= Equals(other.PatternPart, PatternPart);

      return equal;
    }

    /// <summary>
    /// Memento design pattern
    /// </summary>
    public class MementoSearchPattern
    {
      internal MementoSearchPattern(SearchPatter obj)
      {
        PatternPart = obj.PatternPart;
        Pattern = obj.Pattern;
      }

      #region Properties memento

      /// <summary>
      /// Pattern
      /// </summary>
      public Pattern Pattern
      {
        get;
        private set;
      }

      /// <summary>
      /// Pattern part
      /// </summary>
      public Part PatternPart
      {
        get;
        private set;
      }

      #endregion
    }
  }
}
