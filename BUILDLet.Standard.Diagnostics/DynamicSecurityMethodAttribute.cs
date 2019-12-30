// StackFrame.GetMethod() で、最適化の影響を受けずにコールスタックを維持するための回避策
namespace System.Security
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    internal sealed class DynamicSecurityMethodAttribute : Attribute { }
}
