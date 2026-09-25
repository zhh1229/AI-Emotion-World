using TMPro;
using UnityEngine;

namespace AIEmotionWorld.UI
{
    public sealed class ChineseFontAssetProvider : MonoBehaviour
    {
        [SerializeField] private Font sourceFont;

        private TMP_FontAsset runtimeFontAsset;

        public TMP_FontAsset GetFontAsset()
        {
            if (runtimeFontAsset != null)
            {
                return runtimeFontAsset;
            }

            if (sourceFont == null)
            {
                Debug.LogError("Chinese source font is not assigned.", this);
                return null;
            }

            runtimeFontAsset = TMP_FontAsset.CreateFontAsset(
                sourceFont,
                90,
                9,
                UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA,
                1024,
                1024,
                AtlasPopulationMode.Dynamic,
                true);

            if (runtimeFontAsset == null)
            {
                Debug.LogError("Could not create the runtime Chinese TMP font.", this);
                return null;
            }

            runtimeFontAsset.name = "NotoSansSC Runtime SDF";
            return runtimeFontAsset;
        }
    }
}
