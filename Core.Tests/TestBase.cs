namespace Cuplan.Core.Tests;

public class TestBase
{
    private const string RootNamespace = "Cuplan.Core.Tests";

    public TestBase(Type type)
    {
        string? typeNamespace = type.Namespace;
        TestDataPath = $"{Directory.GetCurrentDirectory()}/TestData";

        if (typeNamespace is null) return;

        if (typeNamespace.Length == RootNamespace.Length) return;

        TestDataPath =
            $"{Directory.GetCurrentDirectory()}/TestData/{typeNamespace.Replace($"{RootNamespace}.", "").Replace(".", "/")}/{type.Name}";
    }

    public string TestDataPath { get; }
}