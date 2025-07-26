using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arcane.Carmen
{
    /// <summary>
    /// Records a position within a file.
    /// </summary>
    /// <param name="Line">Line of the file.</param>
    /// <param name="Column">Column on the line.</param>
    /// <param name="Filename">Source file.</param>
    public record Position(int Line, int Column, string Filename);
}
