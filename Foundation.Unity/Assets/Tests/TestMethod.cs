using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// DOTNET Shim
/// </summary>
public class TestMethod : Attribute
{
    public static void RunAll(MonoBehaviour testClass, Action cleanup)
    {
        testClass.StartCoroutine(RunAllAsync(testClass, cleanup));
    }

    public static IEnumerator RunAllAsync(MonoBehaviour testClass, Action cleanup)
    {
        var calls = testClass.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance)
            .Where(
                o => o.GetCustomAttributes(typeof(TestMethod), true)
                    .Any(a => a.GetType() == typeof(TestMethod)))
            .ToArray();

        var name = testClass.GetType().Name;
        Debug.Log(name + ".Start");
        foreach (var call in calls)
        {
            Debug.Log(name + ".Cleanup");
            cleanup();
            Debug.Log(name + "." + call.Name);

            if (call.ReturnType == typeof(IEnumerator))
            {
                yield return testClass.StartCoroutine(call.Name);
            }
            else
            {
                try
                {
                    call.Invoke(testClass, null);
                }
                catch (Exception ex)
                {
                    Debug.LogError("ERROR " + call.Name + " " + ex.Message);
                    UnityEngine.Debug.LogException(ex);
                    yield break;
                }
            }

        }

        cleanup();
        Debug.Log(name + ".End");
        IntegrationTest.Pass(testClass.gameObject);
    }
}