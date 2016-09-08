using System;

/// <summary>
/// DOTNET Shim
/// </summary>
public class Assert
{
    public static void IsNotNull(object o)
    {
        if (o == null)
            throw new Exception("Assertion Failed");
    }

    public static void AreEqual(object a, object b)
    {
        if (!Equals(a, b))
            throw new Exception("Assertion Failed");
    }

    public static void IsNull(object o)
    {
        if (o != null)
            throw new Exception("Assertion Failed");
    }

    public static void IsTrue(bool o)
    {
        if (!o)
            throw new Exception("Assertion Failed");
    }

    public static void IsFalse(bool o)
    {
        if (!o)
            throw new Exception("Assertion Failed");
    }

    public static void Fail(string o)
    {
            throw new Exception(o);
    }

    public static void IsInstanceOfType(object o, Type t)
    {
        if(!o.GetType().IsInstanceOfType(t))
            throw new Exception("Assertion Failed");
    }

}