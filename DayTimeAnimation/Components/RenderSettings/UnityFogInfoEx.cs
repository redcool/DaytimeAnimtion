namespace PowerUtilities
{
    public static class UnityFogInfoEx
    {

        public static void Sync(this UnityFogInfo info, DaytimeRenderSettings p)
        {
            ReflectionTools.CopyFieldInfoValues(p, info);
        }
    }
}
