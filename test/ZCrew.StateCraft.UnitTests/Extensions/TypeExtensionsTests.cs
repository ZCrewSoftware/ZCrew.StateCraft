namespace ZCrew.StateCraft.UnitTests.Extensions;

public class TypeExtensionsTests
{
    [Theory]
    // Built-in type aliases
    [InlineData(typeof(bool), "bool")]
    [InlineData(typeof(byte), "byte")]
    [InlineData(typeof(sbyte), "sbyte")]
    [InlineData(typeof(char), "char")]
    [InlineData(typeof(decimal), "decimal")]
    [InlineData(typeof(double), "double")]
    [InlineData(typeof(float), "float")]
    [InlineData(typeof(int), "int")]
    [InlineData(typeof(uint), "uint")]
    [InlineData(typeof(nint), "nint")]
    [InlineData(typeof(nuint), "nuint")]
    [InlineData(typeof(long), "long")]
    [InlineData(typeof(ulong), "ulong")]
    [InlineData(typeof(short), "short")]
    [InlineData(typeof(ushort), "ushort")]
    [InlineData(typeof(object), "object")]
    [InlineData(typeof(string), "string")]
    [InlineData(typeof(void), "void")]
    // Regular types
    [InlineData(typeof(DateTime), "DateTime")]
    // Arrays
    [InlineData(typeof(int[]), "int[]")]
    [InlineData(typeof(int[,]), "int[,]")]
    [InlineData(typeof(int[,,]), "int[,,]")]
    [InlineData(typeof(int[][]), "int[][]")]
    [InlineData(typeof(string[]), "string[]")]
    // Nullable
    [InlineData(typeof(int?), "int?")]
    [InlineData(typeof(long?), "long?")]
    [InlineData(typeof(DateTime?), "DateTime?")]
    // Generics
    [InlineData(typeof(List<int>), "List<int>")]
    [InlineData(typeof(Dictionary<string, int>), "Dictionary<string, int>")]
    [InlineData(typeof(Dictionary<string, List<int>>), "Dictionary<string, List<int>>")]
    // ValueTuples
    [InlineData(typeof(ValueTuple<int>), "(int)")]
    [InlineData(typeof((int, string)), "(int, string)")]
    [InlineData(typeof((int, string, bool)), "(int, string, bool)")]
    [InlineData(typeof((int, int, int, int, int, int, int)), "(int, int, int, int, int, int, int)")]
    [InlineData(typeof((int, int, int, int, int, int, int, int)), "(int, int, int, int, int, int, int, int)")]
    [InlineData(
        typeof((int, int, int, int, int, int, int, int, int, int)),
        "(int, int, int, int, int, int, int, int, int, int)"
    )]
    [InlineData(typeof((List<int>, Dictionary<string, bool>)), "(List<int>, Dictionary<string, bool>)")]
    // Combined scenarios
    [InlineData(typeof(int?[]), "int?[]")]
    [InlineData(typeof(List<int?>), "List<int?>")]
    [InlineData(typeof((int, string)[]), "(int, string)[]")]
    [InlineData(typeof((int[], string[])), "(int[], string[])")]
    // Pointers
    [InlineData(typeof(int*), "int*")]
    // Open generics
    [InlineData(typeof(List<>), "List<T>")]
    [InlineData(typeof(Dictionary<,>), "Dictionary<TKey, TValue>")]
    public void FriendlyName_WhenGivenType_ShouldReturnExpectedValue(Type type, string expected)
    {
        // Act
        var result = type.FriendlyName;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FriendlyName_WhenGivenByRefType_ShouldReturnRefSyntax()
    {
        // Arrange
        var type = typeof(int).MakeByRefType();

        // Act
        var result = type.FriendlyName;

        // Assert
        Assert.Equal("ref int", result);
    }
}
