using System.Collections.Generic;
using System.Diagnostics;

namespace SpriteEditor
{
    class Sprite
    {
        public string FileName { get; set; }
        public string SafeFileName { get; set; }
        public SpriteMetaData SPRITE_META_DATA { get; set; }
        public SpriteStateDefault SPRITE_STATE_DEFAULT { get; set; }
        private List<SpriteState> st = new List<SpriteState>();
        public List<SpriteState> StateList { get { return st; } }

        public void AddNewState(SpriteState spriteState) { StateList.Add(spriteState); }

        public List<string> GetNonNullProperties(string metaOrDefault, Sprite spr)
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

    class SpriteState : SpriteStateDefault
    {
        public string whichState { get; set; }
        public SpriteState(string stateNameString) { this.whichState = stateNameString; }
        public void SetParameter(string parameterName, string parameterValue)
        {
            switch (parameterName)
            {
                case "uri":
                    this.uri = parameterValue;
                    break;
                case "sizeMultiplier":
                    this.sizeMultiplier = parameterValue;
                    break;
                case "frameDelay":
                    this.frameDelay = parameterValue;
                    break;
                case "cropX":
                    this.cropX = parameterValue;
                    break;
                case "cropY":
                    this.cropY = parameterValue;
                    break;
                case "cropW":
                    this.cropW = parameterValue;
                    break;
                case "cropH":
                    this.cropH = parameterValue;
                    break;
                case "autoClose":
                    this.autoClose = parameterValue;
                    break;
                case "isChain":
                    this.isChain = parameterValue;
                    break;
                case "flipX":
                    this.flipX = parameterValue;
                    break;
                case "sizeDivider":
                    this.sizeDivider = parameterValue;
                    break;
                case "offsX":
                    this.offsX = parameterValue;
                    break;
                case "offsY":
                    this.offsY = parameterValue;
                    break;
                case "usePrevious":
                    this.usePrevious = parameterValue;
                    break;
                case "transparent":
                    this.transparent = parameterValue;
                    break;
                case "walkMultiplier":
                    this.walkMultiplier = parameterValue;
                    break;
                default:
                    Debug.WriteLine("Error: Parameter not found - " + parameterName + ".");
                    break;
            }
        }
    }

    class SpriteMetaData
    {
        public string version { get; set; }
        public List<Credits> credits { get; set; }
        public List<string> actions { get; set; } // walk, death, fly, jump, destroy, run
        public List<string> flags { get; set; } // isCollector, doFadeOut, disablePhysics, disableWindowCollide, disableSpriteCollide, isItem, disableJump
        public List<Spawn> spawn { get; set; }
    }

    class Spawn
    {
        public string uri { get; set; }
        public string spawnX { get; set; }
        public string spawnY { get; set; }
        public string spawnExplode { get; set; }
    }

    class Credits
    {
        public string author { get; set; }
        public string description { get; set; }
        public string url { get; set; }
    }

    class SpriteStateDefault
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