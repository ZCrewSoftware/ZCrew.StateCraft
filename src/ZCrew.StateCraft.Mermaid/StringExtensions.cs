using System.Text;

namespace ZCrew.StateCraft.Mermaid;

/// <summary>
///     Mermaid-specific string extension members.
/// </summary>
internal static class StringExtensions
{
    extension(string text)
    {
        /// <summary>
        ///     Encodes the string for safe inclusion in a Mermaid descriptor: characters that Mermaid would
        ///     otherwise interpret (<c>#lt;</c>, <c>#gt;</c>, runs of spaces) are escaped, and newline characters
        ///     are handled per <see cref="MermaidOptions.Newline"/>.
        /// </summary>
        /// <param name="options">The Mermaid options that control how newline characters are encoded.</param>
        /// <returns>
        ///     The encoded text, or the original string instance when no encoding was required.
        /// </returns>
        public string EncodeForMermaid(MermaidOptions options)
        {
            StringBuilder? builder = null;

            var i = 0;
            for (; i < text.Length; i++)
            {
                var c = text[i];
                switch (c)
                {
                    case '<':
                        ReplaceWithString("#lt;");
                        break;

                    case '>':
                        ReplaceWithString("#gt;");
                        break;

                    // Preserve multiple spaces (which would otherwise be collapsed to a single space) by replacing
                    // subsequent spaces with an HTML space
                    case ' ' when PreviousCharacterIs(' '):
                        ReplaceWithString("#nbsp;");
                        break;

                    // Skipping all newlines (\r, \n, or \r\n)
                    case '\n' when PreviousCharacterIs('\r'):
                    case '\r' or '\n' when options.Newline == MermaidNewline.Ignore:
                        Skip();
                        break;

                    // Replace all newlines (\r or \n) with just a single space
                    case '\r'
                    or '\n' when options.Newline == MermaidNewline.Space:
                        ReplaceWithChar(' ');
                        break;

                    // Replace all newlines (\r or \n) with an HTML single line break (br)
                    case '\r'
                    or '\n' when options.Newline == MermaidNewline.HtmlSingleLineBreak:
                        ReplaceWithString("<br/>");
                        break;

                    // Append to the builder if it is initialized - otherwise the character is appended at that time
                    default:
                        builder?.Append(c);
                        break;
                }
            }

            return builder?.ToString() ?? text;

            void Skip()
            {
                builder ??= new StringBuilder(text[..i]);
            }

            void ReplaceWithChar(char replacement)
            {
                if (builder == null)
                {
                    // Keep the same size since this is a char-for-char replacement
                    builder = new StringBuilder(text[..i], text.Length);
                }
                builder.Append(replacement);
            }

            void ReplaceWithString(string replacement)
            {
                if (builder == null)
                {
                    // The most common replacement will be replacing a single '<' and '>' with "#lt;" and "#gt".
                    // This is why we add 6 characters here so the builder doesn't have to be resized most of the time.
                    // If the assumption is wrong the string builder can grow anyway
                    builder = new StringBuilder(text[..i], text.Length + 6);
                }
                builder.Append(replacement);
            }

            bool PreviousCharacterIs(char c)
            {
                return i > 0 && text[i - 1] == c;
            }
        }
    }
}
