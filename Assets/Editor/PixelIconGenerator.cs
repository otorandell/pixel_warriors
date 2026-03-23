using UnityEngine;
using UnityEditor;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PixelWarriors
{
    public static class PixelIconGenerator
    {
        private const int IconSize = 16;
        private const int Columns = 8;
        private const string TexturePath = "Assets/Resources/Fonts/StatusIcons.png";
        private const string SpriteAssetPath = "Assets/Resources/Fonts/StatusIcons Sprite Asset.asset";

        // Icon names in grid order (left-to-right, top-to-bottom)
        private static readonly string[] IconNames =
        {
            "heart",       "shield",      "sword",       "skull",
            "lightning",   "mana",        "flame",       "snowflake",
            "bleed",       "poison",      "crosshair",   "eye",
            "stun",        "silence",     "terror",      "chain",
            "arrow_up",    "arrow_down",  "cloak",       "fist",
            "star",        "halo",        "pin",         "trap",
            "scope",       "helmet",      "ice_block",   "orb",
            "regen",       "imbue",       "caltrops",    "dodge",
        };

        // Each icon: 16 rows of 16-bit bitmasks (MSB = leftmost pixel)
        private static readonly Dictionary<string, ushort[]> IconData = new()
        {
            ["heart"] = new ushort[]
            {
                0b0000000000000000,
                0b0000000000000000,
                0b0011100111000000,
                0b0111111111100000,
                0b0111111111100000,
                0b0111111111100000,
                0b0111111111100000,
                0b0011111111000000,
                0b0001111110000000,
                0b0000111100000000,
                0b0000011000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["shield"] = new ushort[]
            {
                0b0000000000000000,
                0b0001111110000000,
                0b0011111111000000,
                0b0111111111100000,
                0b0111111111100000,
                0b0111111111100000,
                0b0111111111100000,
                0b0111111111100000,
                0b0011111111000000,
                0b0011111111000000,
                0b0001111110000000,
                0b0000111100000000,
                0b0000011000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["sword"] = new ushort[]
            {
                0b0000000000000000,
                0b0000000001100000,
                0b0000000011100000,
                0b0000000111000000,
                0b0000001110000000,
                0b0000011100000000,
                0b0000111000000000,
                0b0001110000000000,
                0b0011100000000000,
                0b0011010000000000,
                0b0001100000000000,
                0b0000100000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["skull"] = new ushort[]
            {
                0b0000000000000000,
                0b0000111100000000,
                0b0001111110000000,
                0b0011111111000000,
                0b0011011011000000,
                0b0011011011000000,
                0b0011111111000000,
                0b0001111110000000,
                0b0001101110000000,
                0b0000111100000000,
                0b0000101000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["lightning"] = new ushort[]
            {
                0b0000000000000000,
                0b0000011100000000,
                0b0000111000000000,
                0b0001110000000000,
                0b0011100000000000,
                0b0111111100000000,
                0b0001111000000000,
                0b0000110000000000,
                0b0001100000000000,
                0b0011000000000000,
                0b0010000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["mana"] = new ushort[]
            {
                0b0000000000000000,
                0b0000010000000000,
                0b0000110000000000,
                0b0001111000000000,
                0b0001111000000000,
                0b0011111100000000,
                0b0011111100000000,
                0b0011111100000000,
                0b0011111100000000,
                0b0001111000000000,
                0b0000110000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["flame"] = new ushort[]
            {
                0b0000000000000000,
                0b0000010000000000,
                0b0000110000000000,
                0b0000110000000000,
                0b0001111000000000,
                0b0011111100000000,
                0b0011101100000000,
                0b0111101110000000,
                0b0111101110000000,
                0b0111111110000000,
                0b0011111100000000,
                0b0001111000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["snowflake"] = new ushort[]
            {
                0b0000000000000000,
                0b0000010000000000,
                0b0010010010000000,
                0b0001010100000000,
                0b0000111000000000,
                0b0011111110000000,
                0b0000111000000000,
                0b0001010100000000,
                0b0010010010000000,
                0b0000010000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["bleed"] = new ushort[]
            {
                0b0000000000000000,
                0b0010000100000000,
                0b0011001100000000,
                0b0011001100000000,
                0b0011101100000000,
                0b0001111000000000,
                0b0001111000000000,
                0b0000110000000000,
                0b0000000000000000,
                0b0000100000000000,
                0b0001100000000000,
                0b0000100000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["poison"] = new ushort[]
            {
                0b0000000000000000,
                0b0000010000000000,
                0b0000110000000000,
                0b0001111000000000,
                0b0001111000000000,
                0b0011111100000000,
                0b0011111100000000,
                0b0001111000000000,
                0b0000110000000000,
                0b0000000000000000,
                0b0001001000000000,
                0b0000110000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["crosshair"] = new ushort[]
            {
                0b0000000000000000,
                0b0000010000000000,
                0b0000010000000000,
                0b0001111100000000,
                0b0011000110000000,
                0b0010000010000000,
                0b0110010110000000,
                0b0010000010000000,
                0b0011000110000000,
                0b0001111100000000,
                0b0000010000000000,
                0b0000010000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["eye"] = new ushort[]
            {
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000111000000000,
                0b0011111110000000,
                0b0110111011000000,
                0b0110111011000000,
                0b0011111110000000,
                0b0000111000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["stun"] = new ushort[]
            {
                0b0000000000000000,
                0b0010000010000000,
                0b0001000100000000,
                0b0000111000000000,
                0b0001111100000000,
                0b0011111110000000,
                0b0011111110000000,
                0b0001111100000000,
                0b0000111000000000,
                0b0001000100000000,
                0b0010000010000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["silence"] = new ushort[]
            {
                0b0000000000000000,
                0b0000000000000000,
                0b0110000110000000,
                0b0011001100000000,
                0b0001111000000000,
                0b0000110000000000,
                0b0001111000000000,
                0b0011001100000000,
                0b0110000110000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["terror"] = new ushort[]
            {
                0b0000000000000000,
                0b0000111000000000,
                0b0001111100000000,
                0b0011011010000000,
                0b0011111110000000,
                0b0010000010000000,
                0b0011100110000000,
                0b0001111100000000,
                0b0000111000000000,
                0b0000010000000000,
                0b0000010000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["chain"] = new ushort[]
            {
                0b0000000000000000,
                0b0001110000000000,
                0b0010001000000000,
                0b0010001000000000,
                0b0001110000000000,
                0b0001011100000000,
                0b0000100010000000,
                0b0000100010000000,
                0b0000011100000000,
                0b0001110000000000,
                0b0010001000000000,
                0b0010001000000000,
                0b0001110000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["arrow_up"] = new ushort[]
            {
                0b0000000000000000,
                0b0000010000000000,
                0b0000111000000000,
                0b0001111100000000,
                0b0011111110000000,
                0b0000111000000000,
                0b0000111000000000,
                0b0000111000000000,
                0b0000111000000000,
                0b0000111000000000,
                0b0000111000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["arrow_down"] = new ushort[]
            {
                0b0000000000000000,
                0b0000111000000000,
                0b0000111000000000,
                0b0000111000000000,
                0b0000111000000000,
                0b0000111000000000,
                0b0000111000000000,
                0b0011111110000000,
                0b0001111100000000,
                0b0000111000000000,
                0b0000010000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["cloak"] = new ushort[]
            {
                0b0000000000000000,
                0b0000111000000000,
                0b0001111100000000,
                0b0001111100000000,
                0b0000111000000000,
                0b0011111110000000,
                0b0011111110000000,
                0b0011111110000000,
                0b0011111110000000,
                0b0010111010000000,
                0b0010111010000000,
                0b0010000010000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["fist"] = new ushort[]
            {
                0b0000000000000000,
                0b0001101100000000,
                0b0011111110000000,
                0b0011111110000000,
                0b0111111110000000,
                0b0111111100000000,
                0b0111111100000000,
                0b0011111000000000,
                0b0011111000000000,
                0b0001110000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["star"] = new ushort[]
            {
                0b0000000000000000,
                0b0000010000000000,
                0b0000010000000000,
                0b0000111000000000,
                0b0111111110000000,
                0b0011111100000000,
                0b0001111000000000,
                0b0011101100000000,
                0b0011000110000000,
                0b0010000010000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["halo"] = new ushort[]
            {
                0b0000000000000000,
                0b0000111000000000,
                0b0001111100000000,
                0b0001000100000000,
                0b0000111000000000,
                0b0000010000000000,
                0b0001111100000000,
                0b0001111100000000,
                0b0000111000000000,
                0b0001101100000000,
                0b0011000110000000,
                0b0010000010000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["pin"] = new ushort[]
            {
                0b0000000000000000,
                0b0000010000000000,
                0b0000111000000000,
                0b0001111100000000,
                0b0001111100000000,
                0b0000111000000000,
                0b0000010000000000,
                0b0000010000000000,
                0b0000010000000000,
                0b0000010000000000,
                0b0000010000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["trap"] = new ushort[]
            {
                0b0000000000000000,
                0b0000000000000000,
                0b0100000010000000,
                0b0110000110000000,
                0b0011001100000000,
                0b0001111000000000,
                0b0011111100000000,
                0b0111111110000000,
                0b0111111110000000,
                0b0011111100000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["scope"] = new ushort[]
            {
                0b0000000000000000,
                0b0000010000000000,
                0b0001111100000000,
                0b0010000010000000,
                0b0100000001000000,
                0b0100010001000000,
                0b0111110001000000,
                0b0100010001000000,
                0b0100000001000000,
                0b0010000010000000,
                0b0001111100000000,
                0b0000010000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["helmet"] = new ushort[]
            {
                0b0000000000000000,
                0b0000111000000000,
                0b0001111100000000,
                0b0011111110000000,
                0b0011111110000000,
                0b0011111110000000,
                0b0111111111000000,
                0b0110000011000000,
                0b0110000011000000,
                0b0011111110000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["ice_block"] = new ushort[]
            {
                0b0000000000000000,
                0b0001111100000000,
                0b0011111110000000,
                0b0011101110000000,
                0b0011010110000000,
                0b0011101110000000,
                0b0011111110000000,
                0b0011111110000000,
                0b0011101110000000,
                0b0011010110000000,
                0b0011111110000000,
                0b0001111100000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["orb"] = new ushort[]
            {
                0b0000000000000000,
                0b0000000000000000,
                0b0000111000000000,
                0b0001111100000000,
                0b0011110110000000,
                0b0011100110000000,
                0b0011111110000000,
                0b0011111110000000,
                0b0001111100000000,
                0b0000111000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["regen"] = new ushort[]
            {
                0b0000000000000000,
                0b0000000000000000,
                0b0000110000000000,
                0b0000110000000000,
                0b0001111000000000,
                0b0001111000000000,
                0b0000110000000000,
                0b0000110000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["imbue"] = new ushort[]
            {
                0b0000000000000000,
                0b0000000011000000,
                0b0000000110000000,
                0b0000001100000000,
                0b0000011000000000,
                0b0000110000000000,
                0b0001100000000000,
                0b0011000000000000,
                0b0011010000000000,
                0b0001100000000000,
                0b0000100000000000,
                0b0010111010000000,
                0b0001010100000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["caltrops"] = new ushort[]
            {
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000100000000000,
                0b0001010000000000,
                0b0010001000000000,
                0b0000100000000000,
                0b0100010100000000,
                0b0010101010000000,
                0b0001000100000000,
                0b0010001000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
            ["dodge"] = new ushort[]
            {
                0b0000000000000000,
                0b0000000000000000,
                0b0110000000000000,
                0b0011000000000000,
                0b0001100000000000,
                0b0000110000000000,
                0b0000011000000000,
                0b0000001100000000,
                0b0000011000000000,
                0b0000110000000000,
                0b0001100000000000,
                0b0011000000000000,
                0b0110000000000000,
                0b0000000000000000,
                0b0000000000000000,
                0b0000000000000000,
            },
        };

        [MenuItem("Tools/Generate Status Icons Sprite Sheet")]
        public static void GenerateIcons()
        {
            int rows = Mathf.CeilToInt(IconNames.Length / (float)Columns);
            int texWidth = Columns * IconSize;
            int texHeight = rows * IconSize;

            Texture2D texture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;

            // Fill with transparent
            Color[] clearPixels = new Color[texWidth * texHeight];
            texture.SetPixels(clearPixels);

            for (int i = 0; i < IconNames.Length; i++)
            {
                string name = IconNames[i];
                if (!IconData.ContainsKey(name))
                {
                    Debug.LogWarning($"[PixelIconGenerator] No pixel data for icon '{name}', skipping.");
                    continue;
                }

                int col = i % Columns;
                int row = (rows - 1) - (i / Columns); // bottom-up for Unity texture coords
                int baseX = col * IconSize;
                int baseY = row * IconSize;

                ushort[] data = IconData[name];
                for (int py = 0; py < IconSize; py++)
                {
                    ushort rowBits = data[py];
                    for (int px = 0; px < IconSize; px++)
                    {
                        bool filled = (rowBits & (1 << (15 - px))) != 0;
                        if (filled)
                        {
                            // White pixel — will be tinted at runtime
                            texture.SetPixel(baseX + px, baseY + (IconSize - 1 - py), Color.white);
                        }
                    }
                }
            }

            texture.Apply();

            // Ensure directory exists
            string dir = Path.GetDirectoryName(TexturePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            // Save PNG
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(TexturePath, pngData);
            Object.DestroyImmediate(texture);

            AssetDatabase.Refresh();

            // Configure texture importer
            TextureImporter importer = AssetImporter.GetAtPath(TexturePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 256;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;

                // Set sprite sheet metadata
                int rows2 = Mathf.CeilToInt(IconNames.Length / (float)Columns);
                List<SpriteMetaData> spriteSheet = new();
                for (int i = 0; i < IconNames.Length; i++)
                {
                    int col = i % Columns;
                    int row = (rows2 - 1) - (i / Columns);
                    spriteSheet.Add(new SpriteMetaData
                    {
                        name = IconNames[i],
                        rect = new Rect(col * IconSize, row * IconSize, IconSize, IconSize),
                        alignment = (int)SpriteAlignment.Center,
                        pivot = new Vector2(0.5f, 0.5f)
                    });
                }
                importer.spritesheet = spriteSheet.ToArray();
                importer.SaveAndReimport();
            }

            Debug.Log($"[PixelIconGenerator] Generated {IconNames.Length} icons at {TexturePath}");
            Debug.Log("[PixelIconGenerator] Next step: Right-click the texture → Create > TextMeshPro > Sprite Asset");
            Debug.Log("[PixelIconGenerator] Or use Tools > Create Status Icons TMP Sprite Asset");
        }

        [MenuItem("Tools/Create Status Icons TMP Sprite Asset")]
        public static void CreateTMPSpriteAsset()
        {
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath);
            if (tex == null)
            {
                Debug.LogError($"[PixelIconGenerator] Texture not found at {TexturePath}. Run 'Generate Status Icons Sprite Sheet' first.");
                return;
            }

            // Delete old assets if re-running
            if (AssetDatabase.LoadAssetAtPath<Object>(SpriteAssetPath) != null)
                AssetDatabase.DeleteAsset(SpriteAssetPath);

            // Create TMP_SpriteAsset (what TextMeshProUGUI.spriteAsset expects)
            TMPro.TMP_SpriteAsset spriteAsset = ScriptableObject.CreateInstance<TMPro.TMP_SpriteAsset>();
            spriteAsset.name = "StatusIcons Sprite Asset";
            spriteAsset.spriteSheet = tex;

            // Build tables
            int rows = Mathf.CeilToInt(IconNames.Length / (float)Columns);
            var glyphTable = new List<TMPro.TMP_SpriteGlyph>();
            var characterTable = new List<TMPro.TMP_SpriteCharacter>();

            for (int i = 0; i < IconNames.Length; i++)
            {
                int col = i % Columns;
                int row = (rows - 1) - (i / Columns);
                int x = col * IconSize;
                int y = row * IconSize;

                var glyph = new TMPro.TMP_SpriteGlyph
                {
                    index = (uint)i,
                    metrics = new GlyphMetrics(IconSize, IconSize, 0, IconSize * 0.8f, IconSize),
                    glyphRect = new GlyphRect(x, y, IconSize, IconSize),
                    atlasIndex = 0,
                    scale = 1.0f
                };
                glyphTable.Add(glyph);

                var character = new TMPro.TMP_SpriteCharacter(0xFFFE, glyph);
                character.name = IconNames[i];
                character.scale = 1.0f;
                characterTable.Add(character);
            }

            // Use reflection to set the read-only table properties
            // Field names from Debug TMP_SpriteAsset Fields:
            //   m_GlyphTable (not m_SpriteGlyphTable)
            //   m_SpriteCharacterTable
            //   m_Version (must be set to skip UpgradeSpriteAsset)
            SetPrivateField(spriteAsset, "m_GlyphTable", glyphTable);
            SetPrivateField(spriteAsset, "m_SpriteCharacterTable", characterTable);
            SetPrivateField(spriteAsset, "m_Version", "1.1.0");

            // Create material
            Shader spriteShader = Shader.Find("TextMeshPro/Sprite");
            if (spriteShader == null) spriteShader = Shader.Find("Text/Sprite");
            Material mat = new Material(spriteShader);
            mat.mainTexture = tex;
            mat.name = "StatusIcons Sprite Material";
            mat.hideFlags = HideFlags.HideInHierarchy;
            spriteAsset.material = mat;

            // Save
            AssetDatabase.CreateAsset(spriteAsset, SpriteAssetPath);
            AssetDatabase.AddObjectToAsset(mat, spriteAsset);

            // Update lookup tables so sprite-by-name works
            spriteAsset.UpdateLookupTables();

            EditorUtility.SetDirty(spriteAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[PixelIconGenerator] Created TMP Sprite Asset at {SpriteAssetPath} with {IconNames.Length} sprites.");
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            // Walk up the inheritance chain to find the field
            System.Type type = target.GetType();
            while (type != null)
            {
                FieldInfo field = type.GetField(fieldName,
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                if (field != null)
                {
                    field.SetValue(target, value);
                    Debug.Log($"[PixelIconGenerator] Set {fieldName} via reflection on {type.Name}");
                    return;
                }
                type = type.BaseType;
            }
            Debug.LogError($"[PixelIconGenerator] Could not find field '{fieldName}' on {target.GetType().Name} or any base type.");
        }

        /// <summary>
        /// Debug helper: logs all fields on an object and its base types.
        /// </summary>
        [MenuItem("Tools/Debug TMP_SpriteAsset Fields")]
        public static void DebugTMPFields()
        {
            var asset = ScriptableObject.CreateInstance<TMPro.TMP_SpriteAsset>();
            System.Type type = asset.GetType();
            while (type != null)
            {
                Debug.Log($"--- {type.Name} ---");
                foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    Debug.Log($"  {field.FieldType.Name} {field.Name}");
                }
                type = type.BaseType;
                if (type == typeof(Object) || type == typeof(object)) break;
            }
            Object.DestroyImmediate(asset);
        }

        /// <summary>
        /// TMP uses this hash algorithm for sprite name lookups.
        /// Matches the internal TMP_TextParsingUtilities.GetHashCode implementation.
        /// </summary>
        private static int TMP_SpriteNameHash(string s)
        {
            int hash = 0;
            for (int i = 0; i < s.Length; i++)
            {
                hash = ((hash << 5) + hash) ^ char.ToLowerInvariant(s[i]);
            }
            return hash;
        }
    }
}
