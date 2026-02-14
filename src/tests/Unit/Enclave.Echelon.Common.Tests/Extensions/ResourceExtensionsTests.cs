using Enclave.Common.Errors;
using Enclave.Common.Extensions;


namespace Enclave.Echelon.Common.Tests.Extensions;

/// <summary>
/// Unit tests for the <see cref="ResourceExtensions"/> class.
/// </summary>
[UnitTest, TestOf(nameof(ResourceExtensions))]
public class ResourceExtensionsTests
{
    [Fact]
    public void GetResourceStream_WithNullResourcePath_ThrowsArgumentNullException()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => 
            assembly.GetResourceStream(""));
        Should.Throw<ArgumentNullException>(() => 
            assembly.GetResourceStream("   "));
    }

    [Fact]
    public void GetResourceStream_WithNonExistentResource_ReturnsFailure()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var result = assembly.GetResourceStream("non-existent-resource.txt");

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is NotFoundError);
    }

    [Fact]
    public void GetResourceString_WithNonExistentResource_ReturnsFailure()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var result = assembly.GetResourceString("non-existent-resource.txt");

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is NotFoundError);
    }

    [Fact]
    public void GetResourceString_WithType_DelegatesToAssembly()
    {
        // Arrange
        var type = typeof(ResourceExtensionsTests);

        // Act
        var result = type.GetResourceString("non-existent-resource.txt");

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is NotFoundError);
    }

    [Fact]
    public void GetResourceStream_WithType_DelegatesToAssembly()
    {
        // Arrange
        var type = typeof(ResourceExtensionsTests);

        // Act
        var result = type.GetResourceStream("non-existent-resource.txt");

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is NotFoundError);
    }

    [Fact]
    public void GetJsonResource_WithNonExistentResource_ReturnsFailure()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var result = assembly.GetJsonResource<TestData>("non-existent-resource.json");

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is NotFoundError);
    }

    [Fact]
    public void GetJsonResource_WithType_DelegatesToAssembly()
    {
        // Arrange
        var type = typeof(ResourceExtensionsTests);

        // Act
        var result = type.GetJsonResource<TestData>("non-existent-resource.json");

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.ShouldContain(e => e is NotFoundError);
    }

    [Fact]
    public void GetResourceStream_WithExistingResource_ReturnsStream()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act - ResourceExtensions converts - to _, so we use test_text.txt
        // The actual resource is: Enclave.Common.Test.TestResources.test_text.txt
        var result = assembly.GetResourceStream("test_text.txt");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        using var stream = result.Value;
        stream.ShouldNotBeNull();
        stream.CanRead.ShouldBeTrue();
    }

    [Fact]
    public void GetResourceStream_WithExistingResourceUsingSlashes_ReturnsStream()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act - ResourceExtensions converts / to . and - to _
        var result = assembly.GetResourceStream("TestResources/test_text.txt");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }

    [Fact]
    public void GetResourceStream_WithExistingResourceUsingBackslashes_ReturnsStream()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act - ResourceExtensions converts \ to . and - to _
        var result = assembly.GetResourceStream("TestResources\\test_text.txt");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }

    [Fact]
    public void GetResourceStream_WithResourceContainingDashes_ConvertsDashesToUnderscores()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act - ResourceExtensions converts - to _ for matching
        // The actual resource is: Enclave.Common.Test.TestResources.test_resource_with_dashes.json
        // When we search for "test-resource-with-dashes.json", it converts to "test_resource_with_dashes.json"
        // and finds by suffix match
        var result = assembly.GetResourceStream("test-resource-with-dashes.json");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }

    [Fact]
    public void GetResourceStream_WithCaseInsensitiveSearch_FindsResource()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act - Case insensitive search (converts - to _)
        var result = assembly.GetResourceStream("TEST_TEXT.TXT");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }

    [Fact]
    public void GetResourceString_WithExistingResource_ReturnsContent()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var result = assembly.GetResourceString("test-text.txt");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldContain("This is a test text file");
        result.Value.ShouldContain("multiple lines");
    }

    [Fact]
    public void GetResourceString_WithEmptyFile_ReturnsEmptyString()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var result = assembly.GetResourceString("empty_file.txt");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public void GetResourceString_WithType_DelegatesToAssemblyAndReturnsContent()
    {
        // Arrange
        var type = typeof(ResourceExtensionsTests);

        // Act
        var result = type.GetResourceString("test_text.txt");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldContain("This is a test text file");
    }

    [Fact]
    public void GetJsonResource_WithExistingValidJson_ReturnsDeserializedData()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act - ResourceExtensions converts - to _ for matching
        var result = assembly.GetJsonResource<TestData>("test_data.json");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var data = result.Value.ToList();
        data.Count.ShouldBe(2);
        data[0].Value.ShouldBe("Test1");
        data[0].Number.ShouldBe(1);
        data[1].Value.ShouldBe("Test2");
        data[1].Number.ShouldBe(2);
    }

    [Fact]
    public void GetJsonResource_WithType_DelegatesToAssemblyAndReturnsDeserializedData()
    {
        // Arrange
        var type = typeof(ResourceExtensionsTests);

        // Act
        var result = type.GetJsonResource<TestData>("test_data.json");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        var data = result.Value.ToList();
        data.Count.ShouldBe(2);
    }

    [Fact]
    public void GetJsonResource_WithEmptyFile_ThrowsJsonException()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        
        // Act & Assert - Empty file should throw JsonException during deserialization
        // Note: The current implementation doesn't catch JsonException, so it propagates
        Should.Throw<System.Text.Json.JsonException>(() => 
            assembly.GetJsonResource<TestData>("empty_file.txt"));
    }

    [Fact]
    public void GetResourceStream_WithPartialMatch_FindsResourceBySuffix()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act - Using just the filename, should find by suffix match (converts - to _)
        var result = assembly.GetResourceStream("test_text.txt");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        // Should find Enclave.Common.Test.TestResources.test-text.txt by suffix match
    }

    [Fact]
    public void GetResourceStream_WithFullResourceName_FindsResource()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();
        // Get the actual resource name
        var resourceNames = assembly.GetManifestResourceNames();
        var testResource = resourceNames.FirstOrDefault(n => n.Contains("test_text"));

        // Act
        var result = assembly.GetResourceStream(testResource!);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void GetResourceString_WithResourceContainingSpecialCharacters_ReadsCorrectly()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var result = assembly.GetResourceString("test-text.txt");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldContain("Line 3");
        result.Value.ShouldContain("Line 4");
        // Check that newlines are preserved
        result.Value.ShouldContain("\n");
    }

    [Fact]
    public void GetJsonResource_WithResourceContainingDashes_ConvertsAndLoads()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act - The resource name has dashes in the path, which get converted to underscores
        // We can test this by using the converted name
        var result = assembly.GetResourceString("test-resource-with-dashes.json");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldContain("test-key");
        result.Value.ShouldContain("test-value");
    }

    private class TestData
    {
        public string Value { get; set; } = string.Empty;
        public int Number { get; set; }
    }
}
