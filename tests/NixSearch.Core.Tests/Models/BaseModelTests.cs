// SPDX-License-Identifier: MIT

#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.Diagnostics.CodeAnalysis;

using FluentAssertions;

using NixSearch.Core.Models;

namespace NixSearch.Core.Tests.Models;

/// <summary>
/// Tests for <see cref="BaseModel"/>.
/// </summary>
public class BaseModelTests
{
    /// <summary>
    /// Tests that GetPropertyName returns the correct property name for a valid property expression.
    /// </summary>
    [Fact]
    public void GetPropertyName_WithValidPropertyExpression_ShouldReturnPropertyName()
    {
        // Act
        string result = BaseModel.GetPropertyName<NixPackage>(p => p.AttrName);

        // Assert
        result.Should().Be("package_attr_name");
    }

    /// <summary>
    /// Tests that GetPropertyName returns the correct property names for multiple properties.
    /// </summary>
    [Fact]
    public void GetPropertyName_WithMultipleProperties_ShouldReturnCorrectNames()
    {
        // Act
        string attrName = BaseModel.GetPropertyName<NixPackage>(p => p.AttrName);
        string name = BaseModel.GetPropertyName<NixPackage>(p => p.Name);
        string version = BaseModel.GetPropertyName<NixPackage>(p => p.Version);
        string description = BaseModel.GetPropertyName<NixPackage>(p => p.Description);

        // Assert
        attrName.Should().Be("package_attr_name");
        name.Should().Be("package_pname");
        version.Should().Be("package_pversion");
        description.Should().Be("package_description");
    }

    /// <summary>
    /// Tests that GetPropertyName returns the correct property names for NixOption properties.
    /// </summary>
    [Fact]
    public void GetPropertyName_WithNixOptionProperties_ShouldReturnCorrectNames()
    {
        // Act
        string name = BaseModel.GetPropertyName<NixOption>(o => o.Name);
        string description = BaseModel.GetPropertyName<NixOption>(o => o.Description);
        string type = BaseModel.GetPropertyName<NixOption>(o => o.Type);
        string defaultValue = BaseModel.GetPropertyName<NixOption>(o => o.Default);

        // Assert
        name.Should().Be("option_name");
        description.Should().Be("option_description");
        type.Should().Be("option_type");
        defaultValue.Should().Be("option_default");
    }

    /// <summary>
    /// Tests that GetPropertyName returns the correct property name for a nullable property.
    /// </summary>
    [Fact]
    public void GetPropertyName_WithNullableProperty_ShouldReturnPropertyName()
    {
        // Act
        string result = BaseModel.GetPropertyName<NixPackage>(p => p.Description);

        // Assert
        result.Should().Be("package_description");
    }

    /// <summary>
    /// Tests that GetPropertyName returns the correct property name for an array property.
    /// </summary>
    [Fact]
    public void GetPropertyName_WithArrayProperty_ShouldReturnPropertyName()
    {
        // Act
        string result = BaseModel.GetPropertyName<NixPackage>(p => p.Programs);

        // Assert
        result.Should().Be("package_programs");
    }

    /// <summary>
    /// Tests that GetPropertyName returns the correct property name for a complex type property.
    /// </summary>
    [Fact]
    public void GetPropertyName_WithComplexTypeProperty_ShouldReturnPropertyName()
    {
        // Act
        string result = BaseModel.GetPropertyName<NixPackage>(p => p.License);

        // Assert
        result.Should().Be("package_license");
    }

    /// <summary>
    /// Tests that GetPropertyName throws an InvalidOperationException for a property without PropertyName attribute.
    /// </summary>
    [Fact]
    public void GetPropertyName_WithProperty_WithoutAttribute_ShouldThrowInvalidOperationException()
    {
        // Act
        Action act = () => BaseModel.GetPropertyName<TestModelWithoutAttribute>(m => m.PropertyWithoutAttribute);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Property 'PropertyWithoutAttribute' does not have a PropertyName attribute.");
    }

    /// <summary>
    /// Tests that GetPropertyName throws an ArgumentException for an invalid property expression.
    /// </summary>
    [Fact]
    public void GetPropertyName_WithInvalidExpression_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => BaseModel.GetPropertyName<NixPackage>(p => p.ToString());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid property expression*")
            .WithParameterName("expr");
    }

    /// <summary>
    /// Tests that GetPropertyName throws an ArgumentException for a constant expression.
    /// </summary>
    [Fact]
    public void GetPropertyName_WithConstantExpression_ShouldThrowArgumentException()
    {
        // Act
        Action act = () => BaseModel.GetPropertyName<NixPackage>(p => "constant");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("expr");
    }

    /// <summary>
    /// Tests that GetPropertyName returns the correct property names for NixFlake properties.
    /// </summary>
    [Fact]
    public void GetPropertyName_WithNixFlakeProperties_ShouldReturnCorrectNames()
    {
        // Act
        string flakeName = BaseModel.GetPropertyName<NixFlake>(f => f.FlakeName);

        // Assert
        flakeName.Should().Be("flake_name");
    }

    /// <summary>
    /// Tests that GetPropertyName works with different property types.
    /// </summary>
    [Fact]
    public void GetPropertyName_ShouldWork_WithDifferentPropertyTypes()
    {
        // Act
        string stringProp = BaseModel.GetPropertyName<NixPackage>(p => p.Name);
        string arrayProp = BaseModel.GetPropertyName<NixPackage>(p => p.Platforms);
        string nullableProp = BaseModel.GetPropertyName<NixPackage>(p => p.Description);

        // Assert
        stringProp.Should().Be("package_pname");
        arrayProp.Should().Be("package_platforms");
        nullableProp.Should().Be("package_description");
    }

    /// <summary>
    /// Tests that GetPropertyName works with value type properties.
    /// </summary>
    [Fact]
    public void GetPropertyName_WithValueTypeProperty_ShouldReturnPropertyName()
    {
        // This test verifies that the method handles value types (which get boxed in expressions)
        // Act
        string result = BaseModel.GetPropertyName<TestModelWithValueType>(m => m.NumberProperty);

        // Assert
        result.Should().Be("number");
    }

    /// <summary>
    /// Test model without PropertyName attribute.
    /// </summary>
    [ExcludeFromCodeCoverage]
    private sealed record TestModelWithoutAttribute : BaseModel
    {
        public string PropertyWithoutAttribute { get; init; } = string.Empty;
    }

    /// <summary>
    /// Test model with value type property.
    /// </summary>
    [ExcludeFromCodeCoverage]
    private sealed record TestModelWithValueType : BaseModel
    {
        [Nest.PropertyName("number")]
        public int NumberProperty { get; init; }
    }
}