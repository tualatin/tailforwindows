using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


namespace Org.Vs.TailForWin.Utils
{
  /// <summary>
  /// Utility class for checking method arguments. It replaces a lot of methods from {@link ErrorUtil} and makes code more readable as less text is needed.
  /// Author: Stefan From - Dresden Informatik GmbH
  /// </summary>
  public sealed class Arg
  {
    private const string ERR_ATLEAST = "{0} must be at least {1}, but was {2}.";
    private const string ERR_ISGREATER = "{0} must be greater than {1}, but was {2}.";
    private const string ERR_NULL = "{0} must not be null {1}";
    private const string ARG_ARGUMENT_CLASS = "argument class";
    private const string ARG_PATTERN = "pattern";
    private const string ERR_EMPTY = "{0} must not be null or empty.";
    private const string ERR_BLANK = "{0} must not be null or blank.";
    private const string ERR_CONTAINS_EMPTY = "{0} must not contain null or empty values.";
    private const string ERR_CONTAINS_EMPTY_KEY = "{0} must not contain null or empty keys.";

    private Arg()
    {
    }

    /// <summary>
    /// Asserts, that a method argument is not <code>null</code> and throws an ArgumentException if it is <code>null</code>.
    /// </summary>
    /// <param name="T">The type of argument.</param>
    /// <param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    /// <param name="arg">The argument to check.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's.</param>
    /// <returns>The checked argument.</returns>
    public static T NotNull<T>(object source, T arg, string argName)
    {
      if(arg == null)
        throw new ArgumentException(FormatErrorMessage(source, argName, ERR_NULL));

      return (arg);
    }

    /// <summary>
    /// Asserts, that a method argument is not <code>null</code> and throws an {@link ArgumentException} if it is <code>null</code>. The check is only executed, if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    /// <param name="arg">The argument to check.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName"> The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's.
    /// </param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument.</returns>
    public static T NotNullIf<T>(object source, T arg, bool condition, string argName)
    {
      return (condition ? NotNull(source, arg, argName) : arg);
    }

    /**
     * Asserts, that a method argument of type string is not <code>null</code> and not empty and throws an
     * {@link ArgumentException} if it is <code>null</code> or empty.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     * 
     * @see StringUtil#isEmpty(String)
     */
    public static String notEmpty(Object source, String arg, String argName)
    {
      if(StringUtil.isEmpty(arg))
      {
        throw argumentException(source, argName, ERR_EMPTY, ErrorUtil.userReadableName(argName));
      }
      return arg;
    }

    /**
     * Asserts, that a method argument of type string is not <code>null</code> and contains at least one non-whitespace
     * character and throws an {@link ArgumentException} if it is <code>null</code>, empty or contains only whitespace
     * characters.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     * 
     * @see StringUtil#isBlank(String)
     * 
     * @since 0.8
     */
    public static String notBlank(Object source, String arg, String argName)
    {
      if(StringUtil.isBlank(arg))
      {
        throw argumentException(source, argName, ERR_BLANK, ErrorUtil.userReadableName(argName));
      }
      return arg;
    }

    /**
     * Asserts, that a method argument of type string is not <code>null</code> and not empty and throws an
     * {@link ArgumentException} if it is <code>null</code> or empty. The check is only executed, if the given condition
     * is <code>true</code>.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param condition If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     * 
     * @see StringUtil#isEmpty(String)
     */
    public static String notEmptyIf(Object source, String arg, bool condition, String argName)
    {
      return condition ? notEmpty(source, arg, argName) : arg;
    }

    /// <summary>
    /// Asserts, that a method argument of type array is not <code>null</code> and not empty and throws an ArgumentException if it is <code>null</code> or empty.
    /// </summary>
    /// <param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    /// <param name="arg">The argument to check.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument.</returns>
    public static T[] NotEmpty<T>(object source, T[] arg, string argName)
    {
      if(arg == null || arg.Length < 1)
        throw new ArgumentException(FormatErrorMessage(source, argName, ERR_EMPTY));

      return (arg);
    }

    /// <summary>
    /// Asserts, that a method argument of type array is not <code>null</code> and not empty and throws an ArgumentException if it is <code>null</code> or empty. The check is only executed, if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    /// <param name="arg">The argument to check.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument.</returns>
    public static T[] NotEmptyIf<T>(object source, T[] arg, bool condition, string argName)
    {
      return (condition ? NotEmpty(source, arg, argName) : arg);
    }

    /// <summary>
    /// Asserts, that a method argument of type Collection is not <code>null</code> and not empty and throws an ArgumentException if it is <code>null</code> or empty.
    /// </summary>
    /// <param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    /// <param name="arg">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <param name="argName">The argument to check.</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <typeparam name="E">The type of collection</typeparam>
    /// <returns>The checked argument.</returns>
    public static T NotEmpty<T, E>(object source, T arg, string argName) where T : ICollection<E>
    {
      if(arg == null || arg.Count < 1)
        throw new ArgumentException(FormatErrorMessage(source, argName, ERR_EMPTY));

      return (arg);
    }

    /// <summary>
    /// Asserts, that a method argument of type {@link Collection} is not <code>null</code> and not empty and throws an ArgumentException if it is <code>null</code> or empty. The check is only executed, if the given condition
    /// is <code>true</code>.
    /// </summary>
    /// <param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    /// <param name="arg">The argument to check.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <typeparam name="E">The type of collection</typeparam>
    /// <returns>The checked argument.</returns>
    public static T NotEmptyIf<T, E>(object source, T arg, bool condition, string argName) where T : ICollection<E>
    {
      return (condition ? NotEmpty<T, E>(source, arg, argName) : arg);
    }

    /// <summary>
    /// Asserts, that a method argument of type array does not contain <code>null</code> or empty values and throws an ArgumentException if there is a <code>null</code> or empty value.
    /// </summary>
    /// <param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    /// <param name="arg">The argument to check.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument.</returns>
    public static T[] NotContainsEmpty<T>(object source, T[] arg, string argName)
    {
      if(ContainsEmpty<T>(arg))
        throw new ArgumentException(FormatErrorMessage(source, argName, ERR_CONTAINS_EMPTY));

      return (arg);
    }

    /// <summary>
    /// Asserts, that a method argument of type array does not contain <code>null</code> or empty values and throws an ArgumentException} if there is a <code>null</code> or empty value. The check is only executed, 
    /// if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    /// <param name="arg">The argument to check.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <typeparam name="T">The type of argument.</typeparam>
    /// <returns>The checked argument.</returns>
    public static T[] NotContainsEmptyIf<T>(object source, T[] arg, bool condition, string argName)
    {
      return (condition ? NotContainsEmpty(source, arg, argName) : arg);
    }

    /// <summary>
    /// Asserts, that a method argument has a minimum value and throws an ArgumentException if it is lower than the checked value.
    /// </summary>
    /// <param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    /// <param name="arg">The argument to check.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <returns>The checked argument.</returns>
    public static int AtLeast(object source, int arg, int minValue, string argName)
    {
      if(arg < minValue)
        throw new ArgumentException(FormatErrorMessage(source, argName, ERR_ATLEAST, arg, minValue));

      return (arg);
    }

    /// <summary>
    /// Asserts, that a method argument has a minimum value and throws an ArgumentException if it is lower than the checked value. The check is only executed, if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    /// <param name="arg">The argument to check.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's </param>
    /// <returns>The checked argument.</returns>
    public static int AtLeastIf(object source, int arg, int minValue, bool condition, string argName)
    {
      return (condition ? AtLeast(source, arg, minValue, argName) : arg);
    }

    /// <summary>
    /// Asserts, that a method argument has a minimum value and throws an ArgumentException if it is lower than the checked value.
    /// </summary>
    /// <param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    /// <param name="arg">The argument to check.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's</param>
    /// <returns>The checked argument.</returns>
    public static long AtLeast(object source, long arg, long minValue, string argName)
    {
      if(arg < minValue)
        throw new ArgumentException(FormatErrorMessage(source, argName, ERR_ATLEAST, arg, minValue));

      return (arg);
    }

    /// <summary>
    /// Asserts, that a method argument has a minimum value and throws an ArgumentException if it is lower than the checked value. The check is only executed, if the given condition is <code>true</code>.
    /// </summary>
    /// <param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    /// <param name="arg">The argument to check.</param>
    /// <param name="minValue">The minimum value.</param>
    /// <param name="condition">If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.</param>
    /// <param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts of it with the space character in case of a class name. This string is put into the error message.
    /// <code>null</code> or empty, if details about the checked argument must be fetched from source code. The originally given string is set as the exception's </param>
    /// <returns>The checked argument.</returns>
    public static long AtLeastIf(object source, long arg, long minValue, bool condition, string argName)
    {
      return (condition ? AtLeast(source, arg, minValue, argName) : arg);
    }

    /**
     * Asserts, that a method argument is greater than a minimum value and throws an {@link ArgumentException} if it is
     * lower than or equal to the checked value.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param minValue The minimum value.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static int GreaterThan(Object source, int arg, int minValue, String argName)
    {
      if(arg <= minValue)
        throw new ArgumentException(FormatErrorMessage(source, argName, ERR_ISGREATER, arg, minValue));

      return (arg);
    }

    /**
     * Asserts, that a method argument is greater than a minimum value and throws an {@link ArgumentException} if it is
     * lower than or equal to the checked value. The check is only executed, if the given condition is <code>true</code>.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param minValue The minimum value.
     * @param condition If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static int GreaterThanIf(Object source, int arg, int minValue, bool condition, String argName)
    {
      return (condition ? GreaterThan(source, arg, minValue, argName) : arg);
    }

    /**
     * Asserts, that a method argument is greater than a minimum value and throws an {@link ArgumentException} if it is
     * lower than or equal to the checked value.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param minValue The minimum value.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static long GreaterThan(Object source, long arg, long minValue, String argName)
    {
      if(arg <= minValue)
        throw new ArgumentException(FormatErrorMessage(source, argName, ERR_ISGREATER, arg, minValue));

      return (arg);
    }

    /**
     * Asserts, that a method argument is greater than a minimum value and throws an {@link ArgumentException} if it is
     * lower than or equal to the checked value. The check is only executed, if the given condition is <code>true</code>.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param minValue The minimum value.
     * @param condition If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static long GreaterThanIf(Object source, long arg, long minValue, bool condition, String argName)
    {
      return (condition ? GreaterThan(source, arg, minValue, argName) : arg);
    }

    /**
     * Asserts, that a method argument has a maximum value and throws an {@link ArgumentException} if it is greater than
     * the checked value.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param maxValue The maximum value.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static int atMost(Object source, int arg, int maxValue, String argName)
    {
      if(arg > maxValue)
      {
        throw argumentException(
          source,
          argName,
          "{0} must be at most {1}, but was {2}.",
          ErrorUtil.userReadableName(argName),
          maxValue,
          arg);
      }
      return arg;
    }

    /**
     * Asserts, that a method argument has a maximum value and throws an {@link ArgumentException} if it is greater than
     * the checked value. The check is only executed, if the given condition is <code>true</code>.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param maxValue The maximum value.
     * @param condition If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static int atMostIf(Object source, int arg, int maxValue, bool condition, String argName)
    {
      return condition ? atMost(source, arg, maxValue, argName) : arg;
    }

    /**
     * Asserts, that a method argument has a maximum value and throws an {@link ArgumentException} if it is greater than
     * the checked value.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param maxValue The maximum value.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static long atMost(Object source, long arg, long maxValue, String argName)
    {
      if(arg > maxValue)
      {
        throw argumentException(
          source,
          argName,
          "{0} must be at most {1}, but was {2}.",
          ErrorUtil.userReadableName(argName),
          maxValue,
          arg);
      }
      return arg;
    }

    /**
     * Asserts, that a method argument has a maximum value and throws an {@link ArgumentException} if it is greater than
     * the checked value. The check is only executed, if the given condition is <code>true</code>.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param maxValue The maximum value.
     * @param condition If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static long atMostIf(Object source, long arg, long maxValue, bool condition, String argName)
    {
      return condition ? atMost(source, arg, maxValue, argName) : arg;
    }

    /**
     * Asserts, that a method argument is lower than a maximum value and throws an {@link ArgumentException} if it is
     * greater than or equal to the checked value.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param maxValue The maximum value.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static int lowerThan(Object source, int arg, int maxValue, String argName)
    {
      if(arg >= maxValue)
      {
        throw argumentException(
          source,
          argName,
          "{0} must be lower than {1}, but was {2}.",
          ErrorUtil.userReadableName(argName),
          maxValue,
          arg);
      }
      return arg;
    }

    /**
     * Asserts, that a method argument is lower than a maximum value and throws an {@link ArgumentException} if it is
     * greater than or equal to the checked value. The check is only executed, if the given condition is <code>true</code>
     * .
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param maxValue The maximum value.
     * @param condition If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static int lowerThanIf(Object source, int arg, int maxValue, bool condition, String argName)
    {
      return condition ? lowerThan(source, arg, maxValue, argName) : arg;
    }

    /**
     * Asserts, that a method argument is lower than a maximum value and throws an {@link ArgumentException} if it is
     * greater than or equal to the checked value.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param maxValue The maximum value.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static long lowerThan(Object source, long arg, long maxValue, String argName)
    {
      if(arg >= maxValue)
      {
        throw argumentException(
          source,
          argName,
          "{0} must be lower than {1}, but was {2}.",
          ErrorUtil.userReadableName(argName),
          maxValue,
          arg);
      }
      return arg;
    }

    /**
     * Asserts, that a method argument is lower than a maximum value and throws an {@link ArgumentException} if it is
     * greater than or equal to the checked value.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param maxValue The maximum value.
     * @param condition If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static long lowerThanIf(Object source, long arg, long maxValue, bool condition, String argName)
    {
      return condition ? lowerThan(source, arg, maxValue, argName) : arg;
    }

    /**
     * Asserts, that the given argument is of the given class. If the argument is <code>null</code>, then it is never of
     * any given type. This is consistent with Java's <code>instanceof</code> check.
     * 
     * @param <T> The type of argument.
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check. May be <code>null</code>.
     * @param argClass The expected class for the argument. Not <code>null</code>.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument. Not <code>null</code>.
     */
    public static T isA<T>(Object source, Object arg, String argName)
    {
      Type argClass = typeof(T);
      if(arg == null || !(arg is T))
      {
        throw argumentException(
          source,
          argName,
          "{0} was expected to be of class '{1}', but was a '{2}'.",
          ErrorUtil.userReadableName(argName),
          argClass.Name,
          arg != null ? arg.GetType().Name : null);
      }
      return (T) arg;
    }

    /**
     * Asserts, that the given argument class is of the given class. If the argument is <code>null</code>, then it is
     * never of any given type. This is consistent with Java's <code>instanceof</code> check.
     * 
     * @param <T> The type of argument.
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check. May be <code>null</code>.
     * @param argClass The expected class for the argument. Not <code>null</code>.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument. Not <code>null</code>.
     */
    public static Type isA(Object source, Type arg, Type argClass, String argName)
    {
      notNull(typeof(Arg), argClass, ARG_ARGUMENT_CLASS);
      if(arg == null || !argClass.DeclaringType.IsAssignableFrom(arg.DeclaringType))
      {
        throw argumentException(
          source,
          argName,
          "{0} was expected to be of class '{1}', but was a '{2}'.",
          ErrorUtil.userReadableName(argName),
          argClass.Name,
          arg != null ? arg.Name : null);
      }
      return arg;
    }

    /**
     * Asserts, that the given argument is <code>null</code> or of the given class.
     * 
     * @param <T> The type of argument.
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check. May be <code>null</code>.
     * @param argClass The expected class for the argument. Not <code>null</code>.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument. <code>null</code>, if the checked argument is <code>null</code>.
     */
    public static T nullOrIsA<T>(Object source, Object arg, String argName)
    {
      Type argClass = typeof(T);
      if(arg != null)
      {
        isA<T>(source, arg, argName);
      }
      return (T) arg;
    }

    /**
     * Asserts, that the given argument is a subclass of the given class, but NOT the given class itself. If the argument
     * is <code>null</code>, then it is never of any given type. This is consistent with Java's <code>instanceof</code>
     * check.
     * 
     * @param <T> The type of argument.
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param argClass The expected class for the argument. Not <code>null</code>.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument. Not <code>null</code>.
     */
    public static T subclassOf<T>(Object source, Object arg, String argName)
    {
      Type argClass = typeof(T);
      if(!(arg is T) || argClass.Equals(arg.GetType()))
      {
        throw argumentException(
          source,
          argName,
          "{0} was expected to be a subclass of class '{1}', but was a '{2}'.",
          ErrorUtil.userReadableName(argName),
          argClass.Name,
          arg != null ? arg.GetType().Name : null);
      }
      return (T) arg;
    }

    /**
     * Asserts, that the given argument is a subclass of the given class, but not the given class itself. If the argument
     * is <code>null</code>, then it is never of any given type. This is consistent with Java's <code>instanceof</code>
     * check.
     * 
     * @param <T> The type of argument.
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param argClass The expected class for the argument. Not <code>null</code>.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument. Not <code>null</code>.
     */
    public static Type subclassOf(Object source, Type arg, Type argClass, String argName)
    {
      notNull(typeof(Arg), argClass, ARG_ARGUMENT_CLASS);
      if(arg == null || argClass.Equals(arg) || !argClass.IsAssignableFrom(arg))
      {
        throw argumentException(
          source,
          argName,
          "{0} was expected to be a subclass of class '{1}', but was a '{2}'.",
          ErrorUtil.userReadableName(argName),
          argClass.Name,
          arg != null ? arg.Name : null);
      }
      return arg;
    }

    /**
     * Checks that a string argument has one of the values being defined in an enumeration.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param enumClass The enum class defining all possible values. Not <code>null</code>.
     * @return The instantiated enum value from the string. Not <code>null</code>.
     * @throws ArgumentException If the string does not equal any of the possible enumeration values' names.
     */
    public static T oneOf<T>(Object source, String arg) where T : struct, IConvertible
    {
      Type enumClass = typeof(T);

      return oneOf<T>(source, arg, enumClass.Name);
    }

    /**
     * Checks that a string argument has one of the values being defined in an enumeration.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param enumClass The enum class defining all possible values. Not <code>null</code>.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The instantiated enum value from the string. Not <code>null</code>.
     * @throws ArgumentException If the string does not equal any of the possible enumeration values' names.
     */
    public static T oneOf<T>(Object source, String arg, String argName) where T : struct, IConvertible
    {
      Arg.notNull(typeof(Arg), arg, "arg");
      Type enumClass = typeof(T);

      if(arg != null && enumClass.IsEnum)
      {
        try
        {
          return (T) Enum.Parse(enumClass, arg, false);
        }
        catch(ArgumentException e)
        {
          throw argumentException(
            source, argName, e,
            "{0}'s value '{1}' is not valid for {2}. Valid values are {3}.",
            ErrorUtil.userReadableName(argName), arg, enumClass.Name, string.Join(",", enumValues<T>()));
        }
      }
      throw argumentException(
        source, argName,
        "{0}'s value '{1}' is not valid for {2}. Valid values are {3}.",
        ErrorUtil.userReadableName(argName), arg, enumClass.Name, string.Join(",", enumValues<T>()));
    }

    /**
     * Returns all enum values by invoking the static <code>values()</code> method by reflection.
     */
    private static string[] enumValues<T>() where T : struct, IConvertible
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
          values[iEnum] = ((T) fi[iEnum].GetValue(null)).ToString();

          //values[iEnum] = Enum.GetName(enumType, fi[iEnum].GetValue(null));
        }
        //return the array
        return values;
      }

      //the type supplied does not derive from enum
      throw new System.ArgumentException("enumType parameter is not a System.Enum");
    }

    #region HelperFunctions

    private static string FormatErrorMessage(object source, string argumentName, string errorMessage, params object[] arguments)
    {
      return (string.Format(errorMessage, source.GetType().Name, argumentName, arguments));
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
      return (result);
    }

    private static bool IsEmpty<T>(T[] a)
    {
      return (a == null || a.Length == 0);
    }

    #endregion
  }
}