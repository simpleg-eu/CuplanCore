namespace Cuplan.Core.Tests;

public class TestBase
{
    private const string RootNamespace = "Cuplan.Core.Tests";

    public TestBase()
    {
        string? typeNamespace = GetType().Namespace;
        TestDataPath = $"{Directory.GetCurrentDirectory()}/TestData";

        if (typeNamespace is null) return;

        if (typeNamespace.Length == RootNamespace.Length) return;

        TestDataPath =
            $"{Directory.GetCurrentDirectory()}/TestData/{typeNamespace.Replace($"{RootNamespace}.", "").Replace(".", "/")}/{GetType().Name}";
    }

    public string TestDataPath { get; }
}