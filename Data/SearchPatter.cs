using System;
using System.Collections.Generic;
using System.ComponentModel;
using Org.Vs.TailForWin.Utils;


namespace Org.Vs.TailForWin.Data
{
  /// <summary>
  /// Search pattern object 
  /// </summary>
  public class SearchPatter : IDataErrorInfo
  {
    /// <summary>
    /// Standard constructor
    /// </summary>
    public SearchPatter()
    {
      PatternParts = new List<Part>();
    }

    /// <summary>
    /// Is regex pattern
    /// </summary>
    public bool IsRegex
    {
      get;
      set;
    }

    /// <summary>
    /// Pattern
    /// </summary>
    public string Pattern
    {
      get;
      set;
    }

    /// <summary>
    /// List of pattern parts
    /// </summary>
    public List<Part> PatternParts
    {
      get;
      set;
    }

    #region IDataErrorInfo interface

    /// <summary>
    /// Gets the error message for the property with the given name.
    /// </summary>
    /// <param name="columnName">Name of column</param>
    /// <returns>Current error result</returns>
    public string this[string columnName]
    {
      get
      {
        string result = null;

        return (result);
      }
    }

    /// <summary>
    /// Gets an error message indicating what is wrong with this object.
    /// </summary>
    public string Error => throw new NotImplementedException();

    #endregion

    /// <summary>
    /// Create copy of object
    /// </summary>
    /// <returns>A clone of object</returns>
    public SearchPatter Clone()
    {
      return (MemberwiseClone() as SearchPatter);
    }

    /// <summary>
    /// Save data to memenento
    /// </summary>
    /// <returns>Copy of SearchPattern</returns>
    public MementoSearchPattern SaveToMemento()
    {
      return (new MementoSearchPattern(this));
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

      IsRegex = mementoData.IsRegex;
      Pattern = mementoData.Pattern;
      PatternParts = mementoData.PatternParts;
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
        return (false);

      bool equal = true;

      equal &= Equals(other.IsRegex, IsRegex);
      equal &= Equals(other.Pattern, Pattern);
      equal &= Equals(other.PatternParts, PatternParts);

      return (equal);
    }

    /// <summary>
    /// Memento design pattern
    /// </summary>
    public class MementoSearchPattern
    {
      internal MementoSearchPattern(SearchPatter obj)
      {
        IsRegex = obj.IsRegex;
        Pattern = obj.Pattern;
        PatternParts = new List<Part>();

        foreach(var item in obj.PatternParts)
        {
          PatternParts.Add(new Part
          {
            Begin = item.Begin,
            End = item.End
          });
        }        
      }

      #region Properties memento

      /// <summary>
      /// Is regex pattern
      /// </summary>
      public bool IsRegex
      {
        get;
        private set;
      }

      /// <summary>
      /// Pattern
      /// </summary>
      public string Pattern
      {
        get;
        private set;
      }

      /// <summary>
      /// List of pattern parts
      /// </summary>
      public List<Part> PatternParts
      {
        get;
        private set;
      }

      #endregion
    }
  }
}
