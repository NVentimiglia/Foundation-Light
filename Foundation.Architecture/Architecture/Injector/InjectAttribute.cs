// Nicholas Ventimiglia 2016-09-05

using System;

namespace Foundation.Architecture
{
    /// <summary>
    ///     Identifies a service to be injected
    /// </summary>
    /// <remarks>
    ///     Use Inject into
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {
    }
}