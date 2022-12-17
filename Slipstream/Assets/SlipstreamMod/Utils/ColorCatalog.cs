using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slipstream
{
    public static class ColorCatalog
    {
        public static Color ColorRGB(float rUnscaled, float gUnscaled, float bUnscaled, float a = 1f)
        {
            return new Color(rUnscaled / 255f, gUnscaled / 255f, bUnscaled / 255f, a);
        }
    }
}
