// SPDX-License-Identifier: MIT

using System;
using System.Linq.Expressions;
using System.Reflection;

using Nest;

namespace NixSearch.Core.Models;

/// <summary>
/// Base class for all NixSearch models.
/// </summary>
public abstract record BaseModel
{
    /// <summary>
    /// Get `Name` property of `PropertyName` attribute by property with such attribute.
    /// </summary>
    /// <typeparam name="T">Type of the class.</typeparam>
    /// <param name="expr">Expression to get property from.</param>
    /// <returns>Property name from `PropertyName` attribute.</returns>
    /// <exception cref="ArgumentException">Thrown when the expression is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the property does not have a `PropertyName` attribute.</exception>
    public static string GetPropertyName<T>(Expression<Func<T, object?>> expr)
    {
        if (expr.Body is not MemberExpression memberExpression)
        {
            if (expr.Body is UnaryExpression { Operand: MemberExpression unaryMemberExpression })
            {
                memberExpression = unaryMemberExpression;
            }
            else
            {
                throw new ArgumentException("Invalid property expression", nameof(expr));
            }
        }

        PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
        PropertyNameAttribute? attribute = propertyInfo.GetCustomAttribute<PropertyNameAttribute>();

        if (attribute == null)
        {
            throw new InvalidOperationException($"Property '{propertyInfo.Name}' does not have a PropertyName attribute.");
        }

        return attribute.Name;
    }
}
