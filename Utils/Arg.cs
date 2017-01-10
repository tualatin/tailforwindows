using System;
using System.Collections.Generic;
using System.Reflection;


namespace Org.Vs.TailForWin.Utils
{

  /**
   * Utility class for checking method arguments. It replaces a lot of methods from {@link ErrorUtil} and makes code more
   * readable as less text is needed.
   * 
   * @author <a href="mailto:fromm@dresden-informatik.de">Stefan Fromm</a>
   * @version $Rev: 1118 $
   */
  public sealed class Arg
  {
    private const string ARG_ARGUMENT_CLASS = "argument class";
    private const string ARG_PATTERN = "pattern";
    private const string ERR_EMPTY = "{0} must not be null or empty.";
    private const string ERR_BLANK = "{0} must not be null or blank.";
    private const string ERR_CONTAINS_EMPTY = "{0} must not contain null or empty values.";
    private const string ERR_CONTAINS_EMPTY_KEY = "{0} must not contain null or empty keys.";

    private Arg()
    {
    }

    /**
     * Creates an {@link ArgumentException} with a {@linkplain ErrorUtil#machineReadableName(String) machine-readable
     * argument name}. This a convenience method allowing to create argument exceptions with less text.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @param messageKey The translation key for later translation. Not <code>null</code> and not empty. This can be
     *          either a translation key or already a human-understandable error message, which is used as the key.
     * @param messageArgs The arguments for the later translated message. This argument is ignored if the given error
     *          class is not {@link CoreException} or one of its subclasses. <code>null</code> or empty if there are no
     *          arguments.
     * @return The exception. Not <code>null</code>.
     * 
     * @see ErrorUtil#generateUntranslated(Object, Class, String, Object...)
     */
    public static DiEx.ArgumentException argumentException(
      object source,
      string argName,
      string messageKey,
      params object[] messageArgs)
    {
      return argumentException(source, argName, null, messageKey, messageArgs);
    }

    /**
     * Creates an {@link ArgumentException} with a {@linkplain ErrorUtil#machineReadableName(String) machine-readable
     * argument name}. This a convenience method allowing to create argument exceptions with less text.
     * 
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @param cause The cause of the generated error. <code>null</code>, if there is no causing exception.
     * @param messageKey The translation key for later translation. Not <code>null</code> and not empty. This can be
     *          either a translation key or already a human-understandable error message, which is used as the key.
     * @param messageArgs The arguments for the later translated message. This argument is ignored if the given error
     *          class is not {@link CoreException} or one of its subclasses. <code>null</code> or empty if there are no
     *          arguments.
     * @return The exception. Not <code>null</code>.
     * 
     * @see ErrorUtil#generateUntranslated(Object, Class, String, Object...)
     */
    public static DiEx.ArgumentException argumentException(
      object source,
      string argName,
      Exception cause,
      string messageKey,
      params object[] messageArgs)
    {
      return ErrorUtil
          .generateUntranslated<DiEx.ArgumentException>(source, cause, messageKey, messageArgs).setArgumentName(ErrorUtil.machineReadableName(argName));
    }

    ///
    /// Asserts, that a method argument is not <code>null</code> and throws an {@link ArgumentException} if it is
    /// <code>null</code>.
    /// 
    ///<param name="T">The type of argument.</param>
    ///<param name="source">The client object, from which this method is called. Not <code>null</code>.</param>
    ///<param name="arg">The argument to check.</param>
    ///<param name="argName">The name of the argument, which is converted to a user readable string by separating all name parts
    ///          of it with the space character in case of a class name. This string is put into the error message.
    ///          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
    ///          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
    ///          name}.</param>
    ///<returns>The checked argument.</returns>
    public static T notNull<T>(Object source, T arg, String argName)
    {
      if (arg == null)
      {
        throw argumentException(source, argName, "{0} must not be null.", ErrorUtil.userReadableName(argName));
      }
      return arg;
    }

    /**
     * Asserts, that a method argument is not <code>null</code> and throws an {@link ArgumentException} if it is
     * <code>null</code>. The check is only executed, if the given condition is <code>true</code>.
     * 
     * @param <T> The type of argument.
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param condition If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     */
    public static T notNullIf<T>(Object source, T arg, bool condition, String argName)
    {
      return condition ? notNull(source, arg, argName) : arg;
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
      if (StringUtil.isEmpty(arg))
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
      if (StringUtil.isBlank(arg))
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

    ///**
    // * Asserts, than a method argument matches a regular expression white pattern and throws an {@link ArgumentException}
    // * if it does not.
    // * 
    // * @param source The client object, from which this method is called. Not <code>null</code>.
    // * @param arg The argument to check.
    // * @param pattern The uncompiled pattern as defined by {@link Pattern}. Not <code>null</code>. May be empty, if only
    // *          empty argument values are allowed.
    // * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
    // *          of it with the space character in case of a class name. This string is put into the error message.
    // *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
    // *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
    // *          name}.
    // * @return The matcher for the given argument. {@link Matcher#matches()} has been called when this method returns. Not
    // *         <code>null</code>. By calling <code>matcher.group(0)</code> there can be obtained the checked argument
    // *         value.
    // * 
    // * @see #matches(Object, String, Pattern, String)
    // */
    //public static Matcher matches(Object source, String arg, String pattern, String argName)
    //{
    //  notNull(Arg.class, pattern, ARG_PATTERN);

    //  try
    //  {
    //    return matches(source, arg, Pattern.compile(pattern), argName);
    //  }
    //  catch (PatternSyntaxException e)
    //  {
    //    throw argumentException(
    //      Arg.class,
    //      ARG_PATTERN,
    //      e,
    //      "Pattern '{0}' has invalid regular expression syntax.",
    //      pattern);
    //  }
    //}

    ///**
    // * Helper method to reduce paths in {@link #matches(Object, String, Pattern, String)}.
    // */
    //private static void addPatternFlag(Pattern pattern, int flag, String flagName, List<String> allFlags)
    //{
    //  if ( (pattern.flags() & flag) != 0 )
    //  {
    //    allFlags.add(flagName);
    //  }
    //}

    ///**
    // * Asserts, than a method argument matches a white pattern and throws an {@link ArgumentException} if it does not.
    // * 
    // * @param source The client object, from which this method is called. Not <code>null</code>.
    // * @param arg The argument to check.
    // * @param pattern The compiled pattern. Not <code>null</code> and not empty.
    // * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
    // *          of it with the space character in case of a class name. This string is put into the error message.
    // *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
    // *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
    // *          name}.
    // * @return The matcher for the given argument. {@link Matcher#matches()} has been called when this method returns. Not
    // *         <code>null</code>. By calling <code>matcher.group(0)</code> there can be obtained the checked argument
    // *         value.
    // */
    //public static Matcher matches(Object source, String arg, Pattern pattern, String argName)
    //{
    //  notNull(Arg.class, pattern, ARG_PATTERN);

    //  notNull(source, arg, argName);
    //  Matcher matcher = pattern.matcher(arg);
    //  if ( !matcher.matches() )
    //  {
    //    List<String> flags = new ArrayList<String>();
    //    addPatternFlag(pattern, Pattern.UNIX_LINES, "UNIX_LINES", flags);
    //    addPatternFlag(pattern, Pattern.CASE_INSENSITIVE, "CASE_INSENSITIVE", flags);
    //    addPatternFlag(pattern, Pattern.COMMENTS, "COMMENTS", flags);
    //    addPatternFlag(pattern, Pattern.MULTILINE, "MULTILINE", flags);
    //    addPatternFlag(pattern, Pattern.LITERAL, "LITERAL", flags);
    //    addPatternFlag(pattern, Pattern.DOTALL, "DOTALL", flags);
    //    addPatternFlag(pattern, Pattern.UNICODE_CASE, "UNICODE_CASE", flags);
    //    addPatternFlag(pattern, Pattern.CANON_EQ, "CANON_EQ", flags);

    //    throw argumentException(
    //      source,
    //      argName,
    //      "{0} '{1}' does not match the pattern '{2}' with flags {3}.",
    //      ErrorUtil.userReadableName(argName),
    //      arg,
    //      pattern.pattern(),
    //      flags);
    //  }
    //  return matcher;
    //}

    ///**
    // * Asserts, that a method argument of type {@link Map} is not <code>null</code> and not empty and throws an
    // * {@link ArgumentException} if it is <code>null</code> or empty.
    // * 
    // * @param <T> The type of argument.
    // * @param source The client object, from which this method is called. Not <code>null</code>.
    // * @param arg The argument to check.
    // * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
    // *          of it with the space character in case of a class name. This string is put into the error message.
    // *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
    // *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
    // *          name}.
    // * @return The checked argument.
    // * 
    // * @see MapUtil#isEmpty(Map)
    // */
    //public static T notEmpty<T, K, V>(Object source, T arg, String argName) where T : IDictionary<Nullable<K>, Nullable<V>>
    //{
    //  if (arg == null || arg.Count < 1)
    //  {
    //    throw argumentException(source, argName, ERR_EMPTY, ErrorUtil.userReadableName(argName));
    //  }
    //  return arg;
    //}

    ///**
    // * Asserts, that a method argument of type {@link Map} is not <code>null</code> and not empty and throws an
    // * {@link ArgumentException} if it is <code>null</code> or empty. The check is only executed, if the given condition
    // * is <code>true</code>.
    // * 
    // * @param <T> The type of argument.
    // * @param source The client object, from which this method is called. Not <code>null</code>.
    // * @param arg The argument to check.
    // * @param condition If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.
    // * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
    // *          of it with the space character in case of a class name. This string is put into the error message.
    // *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
    // *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
    // *          name}.
    // * @return The checked argument.
    // * 
    // * @see MapUtil#isEmpty(Map)
    // */
    //public static T notEmptyIf<T, K, V>(Object source, T arg, bool condition, String argName) where T : IDictionary<Nullable<K>, Nullable<V>>
    //{
    //  return condition ? notEmpty<T>(source, arg, argName) : arg;
    //}

    /**
     * Asserts, that a method argument of type array is not <code>null</code> and not empty and throws an
     * {@link ArgumentException} if it is <code>null</code> or empty.
     * 
     * @param <T> The type of argument.
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     * 
     * @see ArrayUtil#isEmpty(Object[])
     */
    public static T[] notEmpty<T>(Object source, T[] arg, String argName)
    {
      if (arg == null || arg.Length < 1)
      {
        throw argumentException(source, argName, ERR_EMPTY, ErrorUtil.userReadableName(argName));
      }
      return arg;
    }

    /**
     * Asserts, that a method argument of type array is not <code>null</code> and not empty and throws an
     * {@link ArgumentException} if it is <code>null</code> or empty. The check is only executed, if the given condition
     * is <code>true</code>.
     * 
     * @param <T> The type of argument.
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
     * @see ArrayUtil#isEmpty(Object[])
     */
    public static T[] notEmptyIf<T>(Object source, T[] arg, bool condition, String argName)
    {
      return condition ? notEmpty(source, arg, argName) : arg;
    }

    /**
     * Asserts, that a method argument of type {@link Collection} is not <code>null</code> and not empty and throws an
     * {@link ArgumentException} if it is <code>null</code> or empty.
     * 
     * @param <T> The type of argument.
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     * 
     * @see CollectionUtil#isEmpty(Collection)
     */
    public static T notEmpty<T, E>(Object source, T arg, String argName) where T : ICollection<E>
    {
      if (arg == null || arg.Count < 1)
      {
        throw argumentException(source, argName, ERR_EMPTY, ErrorUtil.userReadableName(argName));
      }
      return arg;
    }

    /**
     * Asserts, that a method argument of type {@link Collection} is not <code>null</code> and not empty and throws an
     * {@link ArgumentException} if it is <code>null</code> or empty. The check is only executed, if the given condition
     * is <code>true</code>.
     * 
     * @param <T> The type of argument.
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
     * @see CollectionUtil#isEmpty(Collection)
     */
    public static T notEmptyIf<T, E>(Object source, T arg, bool condition, String argName) where T : ICollection<E>
    {
      return condition ? notEmpty<T, E>(source, arg, argName) : arg;
    }

    ///**
    // * Asserts, that a method argument of type {@link Map} does not contain <code>null</code> or empty keys and throws an
    // * {@link ArgumentException} if there is a <code>null</code> or empty key.
    // * 
    // * @param <T> The type of argument.
    // * @param source The client object, from which this method is called. Not <code>null</code>.
    // * @param arg The argument to check.
    // * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
    // *          of it with the space character in case of a class name. This string is put into the error message.
    // *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
    // *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
    // *          name}.
    // * @return The checked argument.
    // * 
    // * @see MapUtil#containsEmptyKey(Map)
    // */
    //public static T notContainsEmptyKey<T, K, V>(Object source, T arg, String argName) where T : IDictionary<Nullable<K>, Nullable<V>>
    //{
    //  if (arg == null || arg.ContainsKey(null))
    //  {
    //    throw argumentException(source, argName, ERR_CONTAINS_EMPTY_KEY, ErrorUtil.userReadableName(argName));
    //  }
    //  return arg;
    //}

    ///**
    // * Asserts, that a method argument of type {@link Map} does not contain <code>null</code> or empty keys and throws an
    // * {@link ArgumentException} if there is a <code>null</code> or empty key. The check is only executed, if the given
    // * condition is <code>true</code>.
    // * 
    // * @param <T> The type of argument.
    // * @param source The client object, from which this method is called. Not <code>null</code>.
    // * @param arg The argument to check.
    // * @param condition If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.
    // * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
    // *          of it with the space character in case of a class name. This string is put into the error message.
    // *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
    // *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
    // *          name}.
    // * @return The checked argument.
    // * 
    // * @see MapUtil#containsEmptyKey(Map)
    // */
    //public static T notContainsEmptyKeyIf<T, K, V>(
    //  Object source,
    //  T arg,
    //  bool condition,
    //  String argName) where T : IDictionary<Nullable<K>, Nullable<V>>
    //{
    //  return condition ? notContainsEmptyKey<T, K, V>(source, arg, argName) : arg;
    //}

    ///**
    // * Asserts, that a method argument of type {@link Map} does not contain <code>null</code> or empty values and throws
    // * an {@link ArgumentException} if there is a <code>null</code> or empty value.
    // * 
    // * @param <T> The type of argument.
    // * @param source The client object, from which this method is called. Not <code>null</code>.
    // * @param arg The argument to check.
    // * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
    // *          of it with the space character in case of a class name. This string is put into the error message.
    // *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
    // *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
    // *          name}.
    // * @return The checked argument.
    // * 
    // * @see MapUtil#containsEmptyValue(Map)
    // */
    //public static T notContainsEmptyValue<T, K, V>(Object source, T arg, String argName) where T : IDictionary<Nullable<K>, Nullable<V>>
    //{
    //  notNull(typeof(Arg), arg, "arg");
    //  if (arg.Values.Contains(null))
    //  {
    //    throw argumentException(source, argName, ERR_CONTAINS_EMPTY, ErrorUtil.userReadableName(argName));
    //  }
    //  return arg;
    //}

    ///**
    // * Asserts, that a method argument of type {@link Map} does not contain <code>null</code> or empty values and throws
    // * an {@link ArgumentException} if there is a <code>null</code> or empty value. The check is only executed, if the
    // * given condition is <code>true</code>.
    // * 
    // * @param <T> The type of argument.
    // * @param source The client object, from which this method is called. Not <code>null</code>.
    // * @param arg The argument to check.
    // * @param condition If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.
    // * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
    // *          of it with the space character in case of a class name. This string is put into the error message.
    // *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
    // *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
    // *          name}.
    // * @return The checked argument.
    // * 
    // * @see MapUtil#containsEmptyValue(Map)
    // */
    //public static T notContainsEmptyValueIf<T, K, V>(
    //  Object source,
    //  T arg,
    //  bool condition,
    //  String argName) where T : IDictionary<Nullable<K>, Nullable<V>>
    //{
    //  return condition ? notContainsEmptyValue<T, K, V>(source, arg, argName) : arg;
    //}

    /**
     * Asserts, that a method argument of type array does not contain <code>null</code> or empty values and throws an
     * {@link ArgumentException} if there is a <code>null</code> or empty value.
     * 
     * @param <T> The type of argument.
     * @param source The client object, from which this method is called. Not <code>null</code>.
     * @param arg The argument to check.
     * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
     *          of it with the space character in case of a class name. This string is put into the error message.
     *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
     *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
     *          name}.
     * @return The checked argument.
     * 
     * @see ArrayUtil#containsEmpty(Object[])
     */
    public static T[] notContainsEmpty<T>(Object source, T[] arg, String argName)
    {
      if (ArrayUtil.containsEmpty<T>(arg))
      {
        throw argumentException(source, argName, ERR_CONTAINS_EMPTY, ErrorUtil.userReadableName(argName));
      }
      return arg;
    }

    /**
     * Asserts, that a method argument of type array does not contain <code>null</code> or empty values and throws an
     * {@link ArgumentException} if there is a <code>null</code> or empty value. The check is only executed, if the given
     * condition is <code>true</code>.
     * 
     * @param <T> The type of argument.
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
     * @see ArrayUtil#containsEmpty(Object[])
     */
    public static T[] notContainsEmptyIf<T>(Object source, T[] arg, bool condition, String argName)
    {
      return condition ? notContainsEmpty(source, arg, argName) : arg;
    }

    ///**
    // * Asserts, that a method argument of type {@link Collection} does not contain <code>null</code> or empty values and
    // * throws an {@link ArgumentException} if there is a <code>null</code> or empty value.
    // * 
    // * @param <T> The type of argument.
    // * @param source The client object, from which this method is called. Not <code>null</code>.
    // * @param arg The argument to check.
    // * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
    // *          of it with the space character in case of a class name. This string is put into the error message.
    // *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
    // *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
    // *          name}.
    // * @return The checked argument.
    // * 
    // * @see CollectionUtil#containsEmpty(Collection)
    // */
    //public static T notContainsEmpty<T, E>(Object source, T arg, String argName) where T : ICollection<Nullable<E>>
    //{
    //  if (arg == null || containsEmpty<T, E>(arg))
    //  {
    //    throw argumentException(source, argName, ERR_CONTAINS_EMPTY, ErrorUtil.userReadableName(argName));
    //  }
    //  return arg;
    //}

   // /**
   //* <p>
   //* Checks if any element of a collection is empty. Elements like {@linkplain String strings}, arrays,
   //* {@linkplain Collection collections} and {@linkplain Map maps} are checked with the appropriate utility class of
   //* this package.
   //* </p>
   //* 
   //* <p>
   //* <b>Attention:</b> The given collection must not be contained in its elements or subelements.
   //* </p>
   //* 
   //* @param c The collection to check. Can be <code>null</code>.
   //* @return <code>true</code>, if any of the elements is <code>null</code>. <code>true</code> if any of specially
   //*         handled objects is empty.
   //*/
   // private static bool containsEmpty<T, E>(T c) where T : ICollection<Nullable<E>>
   // {
   //   bool result = false;
   //   if (c == null)
   //   {
   //     return false;
   //   }
   //   for (IEnumerator<E> it = (IEnumerator<E>)c.GetEnumerator(); !result && it.MoveNext(); )
   //   {
   //     E element = it.Current;
   //     if (element == null)
   //     {
   //       result = true;
   //     }
   //     else if (element is ICollection)
   //     {
   //       result = ((ICollection)element).Count < 1;
   //     }
   //     else if (element is IDictionary)
   //     {
   //       result = ((IDictionary)element).Count < 1;
   //     }
   //     else
   //     {
   //       String sElement = element as String;
   //       if (sElement != null)
   //       {
   //         result = StringUtil.isEmpty(sElement);
   //       }
   //       else
   //       {
   //         Object[] oaElement = element as Object[];
   //         if (oaElement != null)
   //         {
   //           result = ArrayUtil.isEmpty(oaElement);
   //         }
   //       }
   //     }

   //   }
   //   return result;
   // }

    ///**
    // * Asserts, that a method argument of type {@link Collection} does not contain <code>null</code> or empty values and
    // * throws an {@link ArgumentException} if there is a <code>null</code> or empty value. The check is only executed, if
    // * the given condition is <code>true</code>.
    // * 
    // * @param <T> The type of argument.
    // * @param source The client object, from which this method is called. Not <code>null</code>.
    // * @param arg The argument to check.
    // * @param condition If <code>true</code>, then the check is made. Otherwise the argument stays unchecked.
    // * @param argName The name of the argument, which is converted to a user readable string by separating all name parts
    // *          of it with the space character in case of a class name. This string is put into the error message.
    // *          <code>null</code> or empty, if details about the checked argument must be fetched from source code. The
    // *          originally given string is set as the exception's {@linkplain ArgumentException#getArgumentName() argument
    // *          name}.
    // * @return The checked argument.
    // * 
    // * @see CollectionUtil#containsEmpty(Collection)
    // */
    //public static T notContainsEmptyIf<T, E>(
    //  Object source,
    //  T arg,
    //  bool condition,
    //  String argName) where T : ICollection<Nullable<E>>
    //{
    //  return condition ? notContainsEmpty<T, E>(source, arg, argName) : arg;
    //}

    /**
     * Asserts, that a method argument has a minimum value and throws an {@link ArgumentException} if it is lower than the
     * checked value.
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
    public static int atLeast(Object source, int arg, int minValue, String argName)
    {
      if (arg < minValue)
      {
        throw argumentException(
          source,
          argName,
          "{0} must be at least {1}, but was {2}.",
          ErrorUtil.userReadableName(argName),
          minValue,
          arg);
      }
      return arg;
    }

    /**
     * Asserts, that a method argument has a minimum value and throws an {@link ArgumentException} if it is lower than the
     * checked value. The check is only executed, if the given condition is <code>true</code>.
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
    public static int atLeastIf(Object source, int arg, int minValue, bool condition, String argName)
    {
      return condition ? atLeast(source, arg, minValue, argName) : arg;
    }

    /**
     * Asserts, that a method argument has a minimum value and throws an {@link ArgumentException} if it is lower than the
     * checked value.
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
    public static long atLeast(Object source, long arg, long minValue, String argName)
    {
      if (arg < minValue)
      {
        throw argumentException(
          source,
          argName,
          "{0} must be at least {1}, but was {2}.",
          ErrorUtil.userReadableName(argName),
          minValue,
          arg);
      }
      return arg;
    }

    /**
     * Asserts, that a method argument has a minimum value and throws an {@link ArgumentException} if it is lower than the
     * checked value. The check is only executed, if the given condition is <code>true</code>.
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
    public static long atLeastIf(Object source, long arg, long minValue, bool condition, String argName)
    {
      return condition ? atLeast(source, arg, minValue, argName) : arg;
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
    public static int greaterThan(Object source, int arg, int minValue, String argName)
    {
      if (arg <= minValue)
      {
        throw argumentException(
          source,
          argName,
          "{0} must be greater than {1}, but was {2}.",
          ErrorUtil.userReadableName(argName),
          minValue,
          arg);
      }
      return arg;
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
    public static int greaterThanIf(Object source, int arg, int minValue, bool condition, String argName)
    {
      return condition ? greaterThan(source, arg, minValue, argName) : arg;
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
    public static long greaterThan(Object source, long arg, long minValue, String argName)
    {
      if (arg <= minValue)
      {
        throw argumentException(
          source,
          argName,
          "{0} must be greater than {1}, but was {2}.",
          ErrorUtil.userReadableName(argName),
          minValue,
          arg);
      }
      return arg;
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
    public static long greaterThanIf(Object source, long arg, long minValue, bool condition, String argName)
    {
      return condition ? greaterThan(source, arg, minValue, argName) : arg;
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
      if (arg > maxValue)
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
      if (arg > maxValue)
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
      if (arg >= maxValue)
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
      if (arg >= maxValue)
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
      if (arg == null || !(arg is T))
      {
        throw argumentException(
          source,
          argName,
          "{0} was expected to be of class '{1}', but was a '{2}'.",
          ErrorUtil.userReadableName(argName),
          argClass.Name,
          arg != null ? arg.GetType().Name : null);
      }
      return (T)arg;
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
      if (arg == null || !argClass.DeclaringType.IsAssignableFrom(arg.DeclaringType))
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
      if (arg != null)
      {
        isA<T>(source, arg, argName);
      }
      return (T)arg;
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
      if (!(arg is T) || argClass.Equals(arg.GetType()))
      {
        throw argumentException(
          source,
          argName,
          "{0} was expected to be a subclass of class '{1}', but was a '{2}'.",
          ErrorUtil.userReadableName(argName),
          argClass.Name,
          arg != null ? arg.GetType().Name : null);
      }
      return (T)arg;
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
      if (arg == null || argClass.Equals(arg) || !argClass.IsAssignableFrom(arg))
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

      if (arg != null && enumClass.IsEnum)
      {
        try
        {
          return (T)Enum.Parse(enumClass, arg, false);
        }
        catch (ArgumentException e)
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
      if (!enumType.IsEnum)
      {
        //get the public static fields (members of the enum)
        FieldInfo[] fi = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
        //create a new enum array
        string[] values = new string[fi.Length];
        //populate with the values
        for (var iEnum = 0; iEnum < fi.Length; iEnum++)
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
  }
}