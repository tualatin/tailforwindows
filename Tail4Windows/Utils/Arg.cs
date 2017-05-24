using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// Utility class for checking method arguments.
  /// Thanks to S.F. from p.
  /// </summary>
  public sealed class Arg
  {
    private const string ERR_ATLEAST = "{0} must be at least {1}, but was {2}.";
    private const string ERR_ISGREATER = "{0} must be greater than {1}, but was {2}.";
    private const string ERR_NULL = "{0} must not be null";
    private const string ERR_ATMOST = "{0} must be at most {1}, but was {2}.";
    private const string ERR_LOWERTHAN = "{0} must be lower than {1}, but was {2}.";
    private const string ARG_ARGUMENT_CLASS = "argument class";
    private const string ERR_EXPECTED_CLASS = "{0} was expected to be of class '{1}', but was a '{2}'.";
    private const string ERR_EXPECTED_SUB_CLASS = "{0} was expected to be a subclass of class '{1}', but was a '{2}'.";
    private const string ERR_EMPTY = "{0} must not be null or empty.";
    private const string ERR_CONTAINS_EMPTY = "{0} must not contain null or empty values.";
    private const string ERR_VALUE_NOT_VALID = "{0}'s value '{1}' is not valid for {2}. Valid values are {3}.";

    private Arg()
    {
    }

    /// <summary>
    /// Asserts, that a method argument is not <code>null</code> and throws an ArgumentException if it is <code>null</code>.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's.</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument.</returns>
    public static T NotNull<T>(T arg, string argName)
    {
      if(arg == null)
        throw new ArgumentException(FormatErrorMessage(argName, ERR_NULL));

      return arg;
    }

    /// <summary>
    /// Asserts, that a method argument is not <code>null</code> and throws an {@link ArgumentException} if it is <code>null</code>. The check is only executed, if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName"> The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's.
    /// </param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument.</returns>
    public static T NotNullIf<T>(T arg, bool condition, string argName)
    {
      return condition ? NotNull(arg, argName) : arg;
    }

    /// <summary>
    /// Asserts, that a method argument of type array is not <code>null</code> and not empty and throws an ArgumentException if it is <code>null</code> or empty.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument.</returns>
    public static T[] NotEmpty<T>(T[] arg, string argName)
    {
      if(arg == null || arg.Length < 1)
        throw new ArgumentException(FormatErrorMessage(argName, ERR_EMPTY));

      return arg;
    }

    /// <summary>
    /// Asserts, that a method argument of type array is not <code>null</code> and not empty and throws an ArgumentException if it is <code>null</code> or empty. The check is only executed, if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument.</returns>
    public static T[] NotEmptyIf<T>(T[] arg, bool condition, string argName)
    {
      return condition ? NotEmpty(arg, argName) : arg;
    }

    /// <summary>
    /// Asserts, that a method argument of type Collection is not <code>null</code> and not empty and throws an ArgumentException if it is <code>null</code> or empty.
    /// </summary>
    /// <param name="arg">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <param name="argName">The argument to check.</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <typeparam name="E">The type of collection</typeparam>
    /// <returns>The checked argument.</returns>
    public static T NotEmpty<T, E>(T arg, string argName) where T : ICollection<E>
    {
      if(arg == null || arg.Count < 1)
        throw new ArgumentException(FormatErrorMessage(argName, ERR_EMPTY));

      return arg;
    }

    /// <summary>
    /// Asserts, that a method argument of type {@link Collection} is not <code>null</code> and not empty and throws an ArgumentException if it is <code>null</code> or empty. The check is only executed, if the given condition
    /// is <code>true</code>.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <typeparam name="E">The type of collection</typeparam>
    /// <returns>The checked argument.</returns>
    public static T NotEmptyIf<T, E>(T arg, bool condition, string argName) where T : ICollection<E>
    {
      return condition ? NotEmpty<T, E>(arg, argName) : arg;
    }

    /// <summary>
    /// Asserts, that a method argument of type array does not contain <code>null</code> or empty values and throws an ArgumentException if there is a <code>null</code> or empty value.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument.</returns>
    public static T[] NotContainsEmpty<T>(T[] arg, string argName)
    {
      if(ContainsEmpty<T>(arg))
        throw new ArgumentException(FormatErrorMessage(argName, ERR_CONTAINS_EMPTY));

      return arg;
    }

    /// <summary>
    /// Asserts, that a method argument of type array does not contain <code>null</code> or empty values and throws an ArgumentException} if there is a <code>null</code> or empty value. The check is only executed, 
    /// if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument.</returns>
    public static T[] NotContainsEmptyIf<T>(T[] arg, bool condition, string argName)
    {
      return condition ? NotContainsEmpty(arg, argName) : arg;
    }

    /// <summary>
    /// Asserts, that a method argument has a minimum value and throws an ArgumentException if it is lower than the checked value.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <returns>The checked argument.</returns>
    public static int AtLeast(int arg, int minValue, string argName)
    {
      if(arg < minValue)
        throw new ArgumentException(FormatErrorMessage(argName, ERR_ATLEAST, arg, minValue));

      return arg;
    }

    /// <summary>
    /// Asserts, that a method argument has a minimum value and throws an ArgumentException if it is lower than the checked value. The check is only executed, if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's </param>
    /// <returns>The checked argument.</returns>
    public static int AtLeastIf(int arg, int minValue, bool condition, string argName)
    {
      return condition ? AtLeast(arg, minValue, argName) : arg;
    }

    /// <summary>
    /// Asserts, that a method argument has a minimum value and throws an ArgumentException if it is lower than the checked value.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <returns>The checked argument.</returns>
    public static long AtLeast(long arg, long minValue, string argName)
    {
      if(arg < minValue)
        throw new ArgumentException(FormatErrorMessage(argName, ERR_ATLEAST, arg, minValue));

      return arg;
    }

    /// <summary>
    /// Asserts, that a method argument has a minimum value and throws an ArgumentException if it is lower than the checked value. The check is only executed, if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's </param>
    /// <returns>The checked argument.</returns>
    public static long AtLeastIf(long arg, long minValue, bool condition, string argName)
    {
      return condition ? AtLeast(arg, minValue, argName) : arg;
    }

    /// <summary>
    /// Asserts, that a method argument is greater than a minimum value and throws an {@link ArgumentException} 
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException if it is lower than or equal to the checked value.</param>
    /// <returns>The checked argument.</returns>
    public static int GreaterThan(int arg, int minValue, string argName)
    {
      if(arg <= minValue)
        throw new ArgumentException(FormatErrorMessage(argName, ERR_ISGREATER, arg, minValue));

      return arg;
    }

    /// <summary>
    /// Asserts, that a method argument is greater than a minimum value and throws an ArgumentException if it is lower than or equal to the checked value. The check is only executed, if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument.</returns>
    public static int GreaterThanIf(int arg, int minValue, bool condition, string argName)
    {
      return condition ? GreaterThan(arg, minValue, argName) : arg;
    }

    /// <summary>
    /// Asserts, that a method argument is greater than a minimum value and throws an ArgumentException if it is lower than or equal to the checked value.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument.</returns>
    public static long GreaterThan(long arg, long minValue, string argName)
    {
      if(arg <= minValue)
        throw new ArgumentException(FormatErrorMessage(argName, ERR_ISGREATER, arg, minValue));

      return arg;
    }

    /// <summary>
    /// Asserts, that a method argument is greater than a minimum value and throws an ArgumentException if it is lower than or equal to the checked value. The check is only executed, if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="minValue"> The minimum value.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument.</returns>
    public static long GreaterThanIf(long arg, long minValue, bool condition, string argName)
    {
      return condition ? GreaterThan(arg, minValue, argName) : arg;
    }

    /// <summary>
    /// Asserts, that a method argument has a maximum value and throws an ArgumentException if it is greater than the checked value.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument.</returns>
    public static int AtMost(int arg, int maxValue, string argName)
    {
      if(arg > maxValue)
        throw new ArgumentException(FormatErrorMessage(argName, ERR_ATMOST, maxValue, arg));

      return arg;
    }

    /// <summary>
    /// Asserts, that a method argument has a maximum value and throws an ArgumentException if it is greater than the checked value. The check is only executed, if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument.</returns>
    public static int AtMostIf(int arg, int maxValue, bool condition, string argName)
    {
      return condition ? AtMost(arg, maxValue, argName) : arg;
    }

    /// <summary>
    /// Asserts, that a method argument has a maximum value and throws an ArgumentException if it is greater than the checked value.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument.</returns>
    public static long AtMost(long arg, long maxValue, string argName)
    {
      if(arg > maxValue)
        throw new ArgumentException(FormatErrorMessage(argName, ERR_ATMOST, maxValue, arg));

      return arg;
    }

    /// <summary>
    /// Asserts, that a method argument has a maximum value and throws an ArgumentException if it is greater than the checked value. The check is only executed, if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument.</returns>
    public static long AtMostIf(long arg, long maxValue, bool condition, string argName)
    {
      return condition ? AtMost(arg, maxValue, argName) : arg;
    }

    /// <summary>
    /// Asserts, that a method argument is lower than a maximum value and throws an ArgumentException if it is greater than or equal to the checked value.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument.</returns>
    public static int LowerThan(int arg, int maxValue, string argName)
    {
      if(arg >= maxValue)
        throw new ArgumentException(FormatErrorMessage(argName, ERR_LOWERTHAN, maxValue, arg));

      return arg;
    }

    /// <summary>
    /// Asserts, that a method argument is lower than a maximum value and throws an {@link ArgumentException} if it is greater than or equal to the checked value. The check is only executed, if the given condition is <code>true</code>
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument.</returns>
    public static int LowerThanIf(int arg, int maxValue, bool condition, string argName)
    {
      return condition ? LowerThan(arg, maxValue, argName) : arg;
    }

    /// <summary>
    /// Asserts, that a method argument is lower than a maximum value and throws an ArgumentException if it is greater than or equal to the checked value.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument.</returns>
    public static long LowerThan(long arg, long maxValue, string argName)
    {
      if(arg >= maxValue)
        throw new ArgumentException(FormatErrorMessage(argName, ERR_LOWERTHAN, maxValue, arg));

      return arg;
    }

    /// <summary>
    /// Asserts, that a method argument is lower than a maximum value and throws an {@link ArgumentException} if it is greater than or equal to the checked value. The check is only executed, if the given condition is <code>true</code>
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="maxValue">The maximum value.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument.</returns>
    public static long LowerThanIf(long arg, long maxValue, bool condition, string argName)
    {
      return condition ? LowerThan(arg, maxValue, argName) : arg;
    }

    /// <summary>
    /// Asserts, that the given argument is of the given class. If the argument is <code>null</code>, then it is never of any given type. This is consistent with Java's <code>instanceof</code> check.
    /// </summary>
    /// <param name="arg">The argument to check. May be <code>null</code>.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument. Not <code>null</code>.</returns>
    public static T IsA<T>(object arg, string argName)
    {
      Type argClass = typeof(T);

      if(arg == null || !(arg is T))
        throw new ArgumentException(FormatErrorMessage(argName, ERR_EXPECTED_CLASS, argClass.Name, arg != null ? arg.GetType().Name : null));

      return (T) arg;
    }

    /// <summary>
    /// Asserts, that the given argument class is of the given class. If the argument is <code>null</code>, then it is never of any given type. This is consistent with Java's <code>instanceof</code> check.
    /// </summary>
    /// <param name="arg">The argument to check. May be <code>null</code>.</param>
    /// <param name="argClass">The expected class for the argument. Not <code>null</code>.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument. Not <code>null</code>.</returns>
    public static Type IsA(Type arg, Type argClass, string argName)
    {
      NotNull(argClass, ARG_ARGUMENT_CLASS);

      if(arg == null || !argClass.DeclaringType.IsAssignableFrom(arg.DeclaringType))
        throw new ArgumentException(FormatErrorMessage(argName, ERR_EXPECTED_CLASS, argClass.Name, arg != null ? arg.Name : null));

      return arg;
    }

    /// <summary>
    /// Asserts, that the given argument is <code>null</code> or of the given class.
    /// </summary>
    /// <param name="arg">The argument to check. May be <code>null</code>.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument. <code>null</code>, if the checked argument is <code>null</code>.</returns>
    public static T NullOrIsA<T>(object arg, string argName)
    {
      if(arg != null)
        IsA<T>(arg, argName);

      return (T) arg;
    }

    /// <summary>
    /// Asserts, that the given argument is a subclass of the given class, but NOT the given class itself. If the argument is <code>null</code>, then it is never of any given type. This is consistent with Java's <code>instanceof</code> check.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument. Not <code>null</code>.</returns>
    public static T SubclassOf<T>(object arg, string argName)
    {
      Type argClass = typeof(T);

      if(!(arg is T) || argClass.Equals(arg.GetType()))
        throw new ArgumentException(FormatErrorMessage(argName, ERR_EXPECTED_SUB_CLASS, argClass.Name, arg != null ? arg.GetType().Name : null));

      return (T) arg;
    }

    /// <summary>
    /// Asserts, that the given argument is a subclass of the given class, but not the given class itself. If the argument is <code>null</code>, then it is never of any given type. This is consistent with Java's <code>instanceof</code> check.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="argClass">The expected class for the argument. Not <code>null</code>.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <returns>The checked argument. Not <code>null</code>.</returns>
    public static Type SubclassOf(Type arg, Type argClass, string argName)
    {
      NotNull(argClass, ARG_ARGUMENT_CLASS);

      if(arg == null || argClass.Equals(arg) || !argClass.IsAssignableFrom(arg))
        throw new ArgumentException(FormatErrorMessage(argName, ERR_EXPECTED_SUB_CLASS, argClass.Name, arg != null ? arg.Name : null));

      return arg;
    }

    /// <summary>
    /// Checks that a string argument has one of the values being defined in an enumeration.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <typeparam name="T">The type of argument</typeparam>
    /// <returns>The instantiated enum value from the string. Not <code>null</code>.</returns>
    public static T OneOf<T>(string arg) where T : struct, IConvertible
    {
      Type enumClass = typeof(T);

      return OneOf<T>(arg, enumClass.Name);
    }

    /// <summary>
    /// Checks that a string argument has one of the values being defined in an enumeration.
    /// </summary>
    /// <param name="arg">The argument to check.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's ArgumentException</param>
    /// <typeparam name="T">The type of argument</typeparam>
    /// <returns>The instantiated enum value from the string. Not <code>null</code>.</returns>
    public static T OneOf<T>(string arg, string argName) where T : struct, IConvertible
    {
      NotNull(arg, "arg");

      Type enumClass = typeof(T);

      if(arg != null && enumClass.IsEnum)
      {
        try
        {
          return (T) Enum.Parse(enumClass, arg, false);
        }
        catch(ArgumentException)
        {
          throw new ArgumentException(FormatErrorMessage(argName, ERR_VALUE_NOT_VALID, enumClass.Name, string.Join(",", EnumValues<T>())));
        }
      }

      throw new ArgumentException(FormatErrorMessage(argName, ERR_VALUE_NOT_VALID, enumClass.Name, string.Join(",", EnumValues<T>())));
    }

    #region HelperFunctions

    /// <summary>
    /// Invoke the static values method by reflection
    /// </summary>
    /// <returns>Returns all enum values by invoking the static <code>values()</code> method by reflection.</returns>
    private static string[] EnumValues<T>() where T : struct, IConvertible
    {
      Type enumType = typeof(T);

      if(!enumType.IsEnum)
      {
        //get the public static fields (members of the enum)
        FieldInfo[] fi = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
        //create a new enum array
        string[] values = new string[fi.Length];

        //populate with the values
        for(var iEnum = 0; iEnum < fi.Length; iEnum++)
        {
          values[iEnum] = ((T) fi[iEnum].GetValue(null)).ToString(CultureInfo.InvariantCulture);

          //values[iEnum] = Enum.GetName(enumType, fi[iEnum].GetValue(null));
        }
        //return the array
        return values;
      }

      //the type supplied does not derive from enum
      throw new ArgumentException("enumType parameter is not a System.Enum");
    }

    private static string FormatErrorMessage(string argumentName, string errorMessage, params object[] arguments)
    {
      return string.Format(errorMessage, argumentName, arguments);
    }

    private static bool ContainsEmpty<T>(T[] a)
    {
      bool result = false;

      if(a != null)
      {
        for(int i = 0; !result && i < a.Length; i++)
        {
          if(a[i] == null)
          {
            result = true;
          }
          else if(a[i] is ICollection)
          {
            result = ((ICollection) a[i]).Count < 1;
          }
          else if(a[i] is IDictionary)
          {
            result = ((IDictionary) a[i]).Count < 1;
          }
          else
          {
            string aString = a as string;

            if(aString != null)
            {
              result = string.IsNullOrEmpty(aString);
            }
            else
            {
              object[] aObjectArray = a as object[];

              if(aObjectArray != null)
                result = IsEmpty(aObjectArray);
            }
          }
        }
      }
      return result;
    }

    private static bool IsEmpty<T>(T[] a)
    {
      return a == null || a.Length == 0;
    }

    #endregion
  }
}