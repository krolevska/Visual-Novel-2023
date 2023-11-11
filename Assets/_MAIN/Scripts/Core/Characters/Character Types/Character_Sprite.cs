using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public class Character_Sprite : Character
    {
        private const string sprite_renderer_parent_name = "Renderers";
        private const string spritesheet_default_name = "Default";
        private const char spritesheet_texture_sprite_delimiter = '-';
        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();

        public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>();

        private string artAssetsDitrectory = "";

        public override bool isVisible => isRevealing || rootCG.alpha > 0;
        public Character_Sprite(string name, CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab)
        {
            rootCG.alpha = enable_on_start ? 1 : 0;
            artAssetsDitrectory = rootAssetsFolder + "/Images";
            GetLayers();
            Debug.Log($"Creates character {name}");

        }

        private void GetLayers()
        {
            Transform rendererRoot = animator.transform.Find(sprite_renderer_parent_name);
            if (rendererRoot == null)
            {
                return;
            }
            for (int i = 0; i < rendererRoot.transform.childCount; i++)
            {
                Transform child = rendererRoot.transform.GetChild(i);
                Image rendererImage = child.GetComponentInChildren<Image>();

                if (rendererImage != null)
                {
                    CharacterSpriteLayer layer = new CharacterSpriteLayer(rendererImage, i);
                    layers.Add(layer);
                    child.name = $"Layer: {i}";
                }
            }
        }

        public void SetSprite(Sprite sprite, int layer = 0)
        {
            layers[layer].SetSprite(sprite);
        }
        public Sprite GetSprite(string spriteName)
        {
            string[] data = spriteName.Split(spritesheet_texture_sprite_delimiter);
            Sprite[] spriteArray = new Sprite[0];

            if (data.Length == 2)
            {
                string textureName = data[0];
                spriteName = data[1];
                spriteArray = Resources.LoadAll<Sprite>($"{artAssetsDitrectory}/{textureName}");

                if (spriteArray.Length == 0)
                {
                    Debug.Log($"Character {name} doesn't have a default art asset called '{textureName}");
                }
            }
            else
            {
                spriteArray = Resources.LoadAll<Sprite>($"{artAssetsDitrectory}/{spritesheet_default_name}");

                if (spriteArray.Length == 0)
                {
                    Debug.Log($"Character {name} doesn't have a default art asset called '{spritesheet_default_name}");
                }

            }
            return Array.Find(spriteArray, sprite => sprite.name == spriteName);
        }
        public Coroutine TransitionSprite(Sprite sprite, int layer = 0, float speed = 1f)
        {
            CharacterSpriteLayer spriteLayer = layers[layer];
            return spriteLayer.TransitionSprite(sprite, speed);
        }
        public override IEnumerator ShowingOrHiding(bool show)
        {
            float targetAlpha = show ? 1f : 0;
            CanvasGroup self = rootCG;

            while (self.alpha < targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime);
                yield return null;
            }
            co_hiding = null;
            co_revealing = null;
        }

        public override void SetColor(Color color)
        {
            base.SetColor(color);
            color = displayColor;
            foreach (CharacterSpriteLayer layer in layers)
            {
                layer.StopChangingColor();
                layer.SetColor(color);
            }
        }
        public override IEnumerator ChangingColor(float speed)
        {
            foreach (CharacterSpriteLayer layer in layers)
            {
                layer.TransitionColor(displayColor, speed);
            }
            yield return null;

            while (layers.Any(l => l.isChangingColor))
            {
                yield return null;
            }
            co_changingColor = null;
        }

        public override IEnumerator Highlighting(float speedMultiplier)
        {
            Color targetColor = displayColor;

            foreach (CharacterSpriteLayer layer in layers)
                layer.TransitionColor(targetColor, speedMultiplier);

            yield return null;

            while (layers.Any(l => l.isChangingColor))
            {
                yield return null;
            }
            co_highlighting = null;
        }

        public override IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            foreach (CharacterSpriteLayer layer in layers)
            {
                if (faceLeft)

                    layer.FaceLeft(speedMultiplier, immediate);

                else
                    layer.FaceRight(speedMultiplier, immediate);
            }
            yield return null;

            while (layers.Any(l => l.isFlipping))
                yield return null;

            co_flipping = null;
        }
    }
}