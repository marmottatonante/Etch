using System.Buffers;

namespace Etch.Drawing;

/// <summary>
/// Represents a draw command.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Draws the command to the output buffer.
    /// </summary>
    /// <param name="buffer">Output buffer.</param>
    void Draw(ArrayBufferWriter<byte> buffer);

    /// <summary>
    /// Undraws the command from the output buffer.
    /// </summary>
    /// <param name="buffer">Output buffer.</param>
    void Undraw(ArrayBufferWriter<byte> buffer);
}