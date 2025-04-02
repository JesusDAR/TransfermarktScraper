using System.Text;
using HtmlAgilityPack;

namespace TransfermarktScraper.ServiceDefaults.Logging.Utils
{
    /// <summary>
    /// Utility class for logging-related functionalities.
    /// </summary>
    public static class LoggingUtils
    {
        /// <summary>
        /// Formats the provided HTML string by loading it into an <see cref="HtmlDocument"/> and returning the indented outer HTML.
        /// </summary>
        /// <param name="html">The HTML string to be formatted.</param>
        /// <returns>A formatted HTML string.</returns>
        public static string FormatHtml(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var stringBuilder = new StringBuilder();
            foreach (var node in doc.DocumentNode.ChildNodes)
            {
                IndentNode(node, stringBuilder, 0);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Recursively indents nodes for better readability.
        /// </summary>
        /// <param name="node">The current HTML node.</param>
        /// <param name="builder">The StringBuilder to append the formatted HTML.</param>
        /// <param name="indentLevel">The current level of indentation.</param>
        private static void IndentNode(HtmlNode node, StringBuilder builder, int indentLevel)
        {
            builder.Append(new string(' ', indentLevel * 2));

            if (node.NodeType == HtmlNodeType.Element)
            {
                builder.Append($"<{node.Name}");

                if (node.HasAttributes)
                {
                    foreach (var attribute in node.Attributes)
                    {
                        builder.Append($" {attribute.Name}=\"{attribute.Value}\"");
                    }
                }

                if (!node.HasChildNodes && !HtmlNode.IsEmptyElement(node.Name))
                {
                    builder.AppendLine(" />");
                }
                else
                {
                    builder.AppendLine(">");

                    if (node.HasChildNodes)
                    {
                        foreach (var child in node.ChildNodes)
                        {
                            IndentNode(child, builder, indentLevel + 1);
                        }
                    }

                    if (!HtmlNode.IsEmptyElement(node.Name))
                    {
                        builder.Append(new string(' ', indentLevel * 2));
                        builder.AppendLine($"</{node.Name}>");
                    }
                }
            }
            else if (node.NodeType == HtmlNodeType.Text)
            {
                builder.AppendLine(node.InnerText.Trim());
            }
        }
    }
}
