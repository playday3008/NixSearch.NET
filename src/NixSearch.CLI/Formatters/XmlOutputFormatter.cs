// SPDX-License-Identifier: MIT

using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Nest;

namespace NixSearch.CLI.Formatters;

/// <summary>
/// XML output formatter.
/// </summary>
/// <typeparam name="T">The type of results to format.</typeparam>
public class XmlOutputFormatter<T> : IOutputFormatter<T>
    where T : class
{
    private static readonly XmlWriterSettings Options = new()
    {
        Indent = true,
        IndentChars = "  ",
        OmitXmlDeclaration = false,
    };

    /// <inheritdoc/>
    public string Format(ISearchResponse<T> response, bool detailed)
    {
        SearchResult<T> result = new()
        {
            Total = response.Total,
            Results = [.. response.Documents],
        };

        XmlSerializer serializer = new(typeof(SearchResult<T>));
        using StringWriter stringWriter = new();
        using XmlWriter xmlWriter = XmlWriter.Create(stringWriter, Options);
        serializer.Serialize(xmlWriter, result);
        return stringWriter.ToString();
    }

    /// <summary>
    /// Wrapper class for serialization.
    /// </summary>
    /// <typeparam name="TResult">The type of results.</typeparam>
    [XmlRoot("SearchResponse")]
    public class SearchResult<TResult>
        where TResult : class
    {
        /// <summary>
        /// Gets or sets the total number of results.
        /// </summary>
        [XmlElement("Total")]
        public long Total { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        [XmlArray("Results")]
        [XmlArrayItem("Item")]
        public TResult[] Results { get; set; } = [];
    }
}
