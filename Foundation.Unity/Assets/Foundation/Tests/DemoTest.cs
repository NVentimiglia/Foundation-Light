using UnityEngine;


[IntegrationTest.DynamicTest("TestScene")]
[IntegrationTest.Timeout(10)]
public class DemoTest : MonoBehaviour
{
    void Start()
    {
        IntegrationTest.Assert(true);

        IntegrationTest.Pass();
    }
}