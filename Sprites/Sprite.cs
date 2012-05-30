using System.Collections.Generic;
using System.Diagnostics;

namespace SpriteEditor
{
    class Sprite
    {
        public string fileName { get; set; }
        public string safeFileName { get; set; }
        public spriteMetaData SPRITE_META_DATA { get; set; }
        public spriteStateDefault SPRITE_STATE_DEFAULT { get; set; }
        private readonly List<SpriteState> _st = new List<SpriteState>();
        public List<SpriteState> stateList { get { return _st; } }

        public void addNewState(SpriteState spriteState) { stateList.Add(spriteState); }

        public List<string> getNonNullProperties(string metaOrDefault, Sprite spr)
        {
            List<string> validProperties = new List<string>();
            if (metaOrDefault.ToLower().Equals("meta")) // S[riteMetaData
            {
                if (spr.SPRITE_META_DATA.version != null)
                    validProperties.Add("version");
                if (spr.SPRITE_META_DATA.credits != null)
                    validProperties.Add("credits");
                if (spr.SPRITE_META_DATA.actions != null)
                    validProperties.Add("actions");
                if (spr.SPRITE_META_DATA.flags != null)
                    validProperties.Add("flags");
                if (spr.SPRITE_META_DATA.spawn != null)
                    validProperties.Add("spawn");
            }
            else // SpriteStateDefault
            {
                if (spr.SPRITE_STATE_DEFAULT.uri != null)
                    validProperties.Add("uri");
                if (spr.SPRITE_STATE_DEFAULT.sizeMultiplier != null)
                    validProperties.Add("sizeMultiplier");
                if (spr.SPRITE_STATE_DEFAULT.frameDelay != null)
                    validProperties.Add("frameDelay");
                if (spr.SPRITE_STATE_DEFAULT.cropX != null)
                    validProperties.Add("cropX");
                if (spr.SPRITE_STATE_DEFAULT.cropY != null)
                    validProperties.Add("cropY");
                if (spr.SPRITE_STATE_DEFAULT.cropW != null)
                    validProperties.Add("cropW");
                if (spr.SPRITE_STATE_DEFAULT.cropH != null)
                    validProperties.Add("cropH");
                if (spr.SPRITE_STATE_DEFAULT.autoClose != null)
                    validProperties.Add("autoClose");
                if (spr.SPRITE_STATE_DEFAULT.isChain != null)
                    validProperties.Add("isChain");
                if (spr.SPRITE_STATE_DEFAULT.flipX != null)
                    validProperties.Add("flipX");
                if (spr.SPRITE_STATE_DEFAULT.sizeDivider != null)
                    validProperties.Add("sizeDivider");
                if (spr.SPRITE_STATE_DEFAULT.offsX != null)
                    validProperties.Add("offsX");
                if (spr.SPRITE_STATE_DEFAULT.offsY != null)
                    validProperties.Add("offsY");
                if (spr.SPRITE_STATE_DEFAULT.usePrevious != null)
                    validProperties.Add("usePrevious");
                if (spr.SPRITE_STATE_DEFAULT.transparent != null)
                    validProperties.Add("transparent");
                if (spr.SPRITE_STATE_DEFAULT.walkMultiplier != null)
                    validProperties.Add("walkMultiplier");
                if (spr.SPRITE_STATE_DEFAULT.fixtures != null)
                    validProperties.Add("fixtures");
            }
            return validProperties;
        }
    }

    class SpriteState : spriteStateDefault
    {
        public string whichState { get; set; }
        public SpriteState(string stateNameString) { whichState = stateNameString; }
        public void setParameter(string parameterName, string parameterValue)
        {
            switch (parameterName)
            {
                case "uri":
                    uri = parameterValue;
                    break;
                case "sizeMultiplier":
                    sizeMultiplier = parameterValue;
                    break;
                case "frameDelay":
                    frameDelay = parameterValue;
                    break;
                case "cropX":
                    cropX = parameterValue;
                    break;
                case "cropY":
                    cropY = parameterValue;
                    break;
                case "cropW":
                    cropW = parameterValue;
                    break;
                case "cropH":
                    cropH = parameterValue;
                    break;
                case "autoClose":
                    autoClose = parameterValue;
                    break;
                case "isChain":
                    isChain = parameterValue;
                    break;
                case "flipX":
                    flipX = parameterValue;
                    break;
                case "sizeDivider":
                    sizeDivider = parameterValue;
                    break;
                case "offsX":
                    offsX = parameterValue;
                    break;
                case "offsY":
                    offsY = parameterValue;
                    break;
                case "usePrevious":
                    usePrevious = parameterValue;
                    break;
                case "transparent":
                    transparent = parameterValue;
                    break;
                case "walkMultiplier":
                    walkMultiplier = parameterValue;
                    break;
                default:
                    Debug.WriteLine("Error: Parameter not found - " + parameterName + ".");
                    break;
            }
        }
    }

    class spriteMetaData
    {
        public string version { get; set; }
        public List<credits> credits { get; set; }
        public List<string> actions { get; set; } // walk, death, fly, jump, destroy, run
        public List<string> flags { get; set; } // isCollector, doFadeOut, disablePhysics, disableWindowCollide, disableSpriteCollide, isItem, disableJump
        public List<spawn> spawn { get; set; }
    }

    class spawn
    {
        public string uri { get; set; }
        public string spawnX { get; set; }
        public string spawnY { get; set; }
        public string spawnExplode { get; set; }
    }

    class credits
    {
        public string author { get; set; }
        public string description { get; set; }
        public string url { get; set; }
    }

    class spriteStateDefault
    {
        public string uri { get; set; }
        public string sizeMultiplier { get; set; }
        public string frameDelay { get; set; }
        public string cropX { get; set; }
        public string cropY { get; set; }
        public string cropW { get; set; }
        public string cropH { get; set; }
        public string autoClose { get; set; }
        public string isChain { get; set; }
        public string flipX { get; set; }
        public string sizeDivider { get; set; }
        public string offsX { get; set; }
        public string offsY { get; set; }
        public string usePrevious { get; set; }
        public string transparent { get; set; }
        public string walkMultiplier { get; set; }
        public List<fixtures> fixtures { get; set; }
    }

    class fixtures
    {
        public string x { get; set; }
        public string y { get; set; }
        public string w { get; set; }
        public string h { get; set; }
    }

}