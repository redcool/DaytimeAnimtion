using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerUtilities
{
    public static class AmbientInfoEx
    {

        /// <summary>
        /// Apply DaytimeForwardParams's params
        /// </summary>
        /// <param name="p"></param>
        public static void Sync(this AmbientInfo info ,DaytimeRenderSettings p)
        {
            //isUpdateAmbient = p.isUpdateAmbient;
            //ambientSkyColor = p.ambientSkyColor;
            //ambientEquatorColor = p.ambientEquatorColor;
            //ambientGroundColor = p.ambientGroundColor;

            ReflectionTools.CopyFieldInfoValues(p, info);
        }
    }
}
