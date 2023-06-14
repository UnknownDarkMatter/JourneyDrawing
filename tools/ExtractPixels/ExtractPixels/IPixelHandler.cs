using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPixels;

public interface IPixelHandler
{
    public void AddPixel(int x, int y, int continentNumber, ref int s);
}
