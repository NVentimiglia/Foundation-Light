using System;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// DOTNET Shim
/// </summary>
public class Assert
{
    public static string GetMethodName()
    {
        // get call stack
        var stackTrace = new StackTrace();
        // get method calls (frames)
        var stackFrames = stackTrace.GetFrames().ToList();

        string n = String.Empty;
        for (int i = 0; i < stackFrames.Count; i++)
        {
            if (i == 0)
                continue;

            n = stackFrames[i].GetMethod().Name;

            if (n == "MoveNext")
                continue;

            break;
        }
        return n;
    }

    public static void IsNotNull(object o)
    {
       
        if (o == null)
            throw new Exception("IsNotNull Assertion Failed on " + GetMethodName());
    }

    public static void AreEqual<T>(T a, T b)
    {
        if (!a.Equals(b))
            throw new Exception("AreEqual Assertion Failed on " + GetMethodName());
    }

    public static void IsNull(object o)
    {
        if (o != null)
            throw new Exception("IsNull Assertion Failed on " + GetMethodName());
    }

    public static void IsTrue(bool o)
    {
        if (!o)
            throw new Exception("IsTrue Assertion Failed on " + GetMethodName());
    }

    public static void IsFalse(bool o)
    {
        if (!o)
            throw new Exception("IsFalse Assertion Failed on " + GetMethodName());
    }

    public static void Fail(string o)
    {
        throw new Exception(o);
    }

    public static void IsInstanceOfType(object o, Type t)
    {
        if (o.GetType() != t)
            throw new Exception("IsInstanceOfType Assertion Failed on " + GetMethodName());
    }

}