namespace Api;

/// <summary>
/// Marker type used to locate the Api assembly (e.g. by architecture tests and DI scanning).
/// </summary>
public static class AssemblyReference
{
    public static readonly System.Reflection.Assembly Assembly = typeof(AssemblyReference).Assembly;
}
