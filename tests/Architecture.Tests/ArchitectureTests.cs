using System.Reflection;

namespace Architecture.Tests;

/// <summary>
/// Enforces the Clean Architecture dependency rule: dependencies may only point inward.
///
///   Api  ──►  Infrastructure  ──►  Application  ──►  Domain
///
/// Domain        → depends on nothing.
/// Application   → may depend on Domain only.
/// Infrastructure→ may depend on Application and Domain, never on Api.
/// Api           → composition root, may depend on everything.
/// </summary>
public class ArchitectureTests
{
    private static readonly Assembly DomainAssembly = Domain.AssemblyReference.Assembly;
    private static readonly Assembly ApplicationAssembly = Application.AssemblyReference.Assembly;
    private static readonly Assembly InfrastructureAssembly = Infrastructure.AssemblyReference.Assembly;
    private static readonly Assembly ApiAssembly = Api.AssemblyReference.Assembly;

    // Some files use legacy root namespaces (e.g. "Sakani.API", "WebApi", "Sakani.Infrastructure"),
    // so every rule checks all the namespaces a layer is known to use.
    private static readonly string[] ApplicationNamespaces = { "Application" };
    private static readonly string[] InfrastructureNamespaces = { "Infrastructure", "Sakani.Infrastructure" };
    private static readonly string[] ApiNamespaces = { "Api", "Sakani.API", "WebApi" };

    // ── Project (assembly) references ────────────────────────────────────────

    [Fact]
    public void Domain_Should_NotReference_AnyOtherProject()
    {
        ReferencedProjects(DomainAssembly).Should().BeEmpty();
    }

    [Fact]
    public void Application_Should_OnlyReference_Domain()
    {
        ReferencedProjects(ApplicationAssembly).Should().BeEquivalentTo(DomainAssembly.GetName().Name);
    }

    [Fact]
    public void Infrastructure_Should_NotReference_Api()
    {
        ReferencedProjects(InfrastructureAssembly).Should().NotContain(ApiAssembly.GetName().Name);
    }

    // ── Type-level dependencies (catches usages, not just references) ────────

    [Fact]
    public void Domain_Should_NotHaveDependencyOn_OtherLayers()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny([.. ApplicationNamespaces, .. InfrastructureNamespaces, .. ApiNamespaces])
            .GetResult();

        AssertSuccessful(result);
    }

    [Fact]
    public void Application_Should_NotHaveDependencyOn_InfrastructureOrApi()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny([.. InfrastructureNamespaces, .. ApiNamespaces])
            .GetResult();

        AssertSuccessful(result);
    }

    [Fact]
    public void Infrastructure_Should_NotHaveDependencyOn_Api()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApiNamespaces)
            .GetResult();

        AssertSuccessful(result);
    }

    [Fact]
    public void Domain_Should_NotHaveDependencyOn_EntityFrameworkCore()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        AssertSuccessful(result);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static readonly HashSet<string?> SolutionProjects = new[]
    {
        DomainAssembly, ApplicationAssembly, InfrastructureAssembly, ApiAssembly,
    }.Select(a => a.GetName().Name).ToHashSet();

    private static IEnumerable<string?> ReferencedProjects(Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(a => a.Name)
            .Where(SolutionProjects.Contains);

    private static void AssertSuccessful(TestResult result) =>
        result.IsSuccessful.Should().BeTrue(
            "these types break the dependency rule: {0}",
            string.Join(", ", result.FailingTypeNames ?? []));
}
