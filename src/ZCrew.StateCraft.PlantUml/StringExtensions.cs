using System.Text;

namespace ZCrew.StateCraft.PlantUml;

/// <summary>
///     PlantUML-specific string extension members.
/// </summary>
internal static class StringExtensions
{
    extension(string text)
    {
        /// <summary>
        ///     Encodes the string for safe inclusion in a PlantUML descriptor: characters PlantUML would otherwise
        ///     interpret as creole markup (<c>&lt;</c>, <c>&gt;</c>, runs of spaces) are replaced with their unicode
        ///     escapes, and newline characters are handled per <see cref="PlantUmlOptions.Newline"/>.
        /// </summary>
        /// <param name="options">The PlantUML options that control how newline characters are encoded.</param>
        /// <returns>
        ///     The encoded text, or the original string instance when no encoding was required.
        /// </returns>
        public string EncodeForPlantUml(PlantUmlOptions options)
        {
            StringBuilder? builder = null;

            var i = 0;
            for (; i < text.Length; i++)
            {
                var c = text[i];
                switch (c)
                {
                    // PlantUML parses a set of HTML-like tags inside labels, so angle brackets are escaped to their
                    // unicode form rather than emitted literally
                    case '<':
                        ReplaceWithString("<U+003C>");
                        break;

                    case '>':
                        ReplaceWithString("<U+003E>");
                        break;

                    // Preserve multiple spaces (which would otherwise be collapsed to a single space) by replacing
                    // subsequent spaces with a non-breaking space
                    case ' ' when PreviousCharacterIs(' '):
                        ReplaceWithString("<U+00A0>");
                        break;

                    // Skipping all newlines (\r, \n, or \r\n)
                    case '\n' when PreviousCharacterIs('\r'):
                    case '\r' or '\n' when options.Newline == PlantUmlNewline.Ignore:
                        Skip();
                        break;

                    // Replace all newlines (\r or \n) with just a single space
                    case '\r'
                    or '\n' when options.Newline == PlantUmlNewline.Space:
                        ReplaceWithChar(' ');
                        break;

                    // Replace all newlines (\r or \n) with PlantUML's in-label line break escape
                    case '\r'
                    or '\n' when options.Newline == PlantUmlNewline.LineBreak:
                        ReplaceWithString("\\n");
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
                    // The most common replacement will be replacing a single '<' and '>' with their unicode escapes,
                    // each of which grows the string by 7 characters. This is why we add 14 characters here so the
                    // builder doesn't have to be resized most of the time. If the assumption is wrong the string
                    // builder can grow anyway
                    builder = new StringBuilder(text[..i], text.Length + 14);
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
