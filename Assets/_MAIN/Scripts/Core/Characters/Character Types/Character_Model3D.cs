using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Characters
{
    public class Character_Model3D : Character
    {
        private const string character_render_group_prefab_name_format = "RenderGroup - [{0}]";
        private const string character_render_texture_name_format = "RenderTextureModel3D";
        private const int character_stacking_depth = 15;
        private const float expression_transition_speed = 100f;
        private const float default_transition_speed = 3f;
        private const float default_facing_direction_value = 25f;

        private GameObject renderGroup;
        private Camera camera;
        private Transform modelContainer, model;
        private Animator modelAnimator;
        private SkinnedMeshRenderer modelExpressionController;

        private RawImage renderer;
        private CanvasGroup rendererCG => renderer.GetComponent<CanvasGroup>();
        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();

        private Dictionary<string, Coroutine> expressionCoroutines;

        public override bool isVisible { get => isRevealing || rootCG.alpha > 0; set => rootCG.alpha = value ? 1 : 0; }

        private Coroutine co_fadingOutOldRenderers = null;
        private bool isFadingOutOldRenderers => co_fadingOutOldRenderers != null;
        private float oldRendererFadeOutSpeedMultiplier = default_transition_speed;

        private struct OldRenderer
        {

            public CanvasGroup oldCG;
            public RawImage oldImage;
            public GameObject oldRenderGroup;

            public OldRenderer(CanvasGroup oldCG, RawImage oldImage, GameObject oldRenderGroup)
            {
                this.oldCG = oldCG;
                this.oldImage = oldImage;
                this.oldRenderGroup = oldRenderGroup;
            }
        }
        private List<OldRenderer> oldRenderers = new List<OldRenderer>();
        public Character_Model3D(string name, CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab)
        {
            Debug.Log($"Creates character {name}");

            GameObject renderGroupPrefab = Resources.Load<GameObject>(rootAssetsFolder + '/' + string.Format(character_render_group_prefab_name_format, config.name));
            renderGroup = Object.Instantiate(renderGroupPrefab, characterManager.characterPanelModel3D);
            renderGroup.name = string.Format(character_render_group_prefab_name_format, name);
            renderGroup.SetActive(true);
            camera = renderGroup.GetComponentInChildren<Camera>();
            modelContainer = camera.transform.GetChild(0);
            model = modelContainer.GetChild(0);
            modelAnimator = model.GetComponent<Animator>();
            modelExpressionController = model.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(sm => sm.sharedMesh.blendShapeCount > 0);

            renderer = animator.GetComponentInChildren<RawImage>();
            RenderTexture renderTex = Resources.Load<RenderTexture>(rootAssetsFolder + '/' + character_render_texture_name_format);
            RenderTexture newTex = new RenderTexture(renderTex);
            renderer.texture = newTex;
            camera.targetTexture = newTex;
            int modelsInScene = characterManager.GetCharacterCountFromCharacterType(CharacterType.Model3D);
            renderGroup.transform.position += Vector3.down * (character_stacking_depth + modelsInScene);
        }

        public void SetMotion(string motionName)
        {
            modelAnimator.Play(motionName);
        }

        public void SetExpression(string blendShapeName, float weight, float speedMultiplier = 0, bool immediate = false)
        {
            if (modelExpressionController == null)
            {
                Debug.Log($"Character {name} does not have an expression controller");
                return;
            }

            if (expressionCoroutines.ContainsKey(blendShapeName))
            {
                characterManager.StopCoroutine(expressionCoroutines[blendShapeName]);
                expressionCoroutines.Remove(blendShapeName);
            }
            Coroutine expressionCoroutine = characterManager.StartCoroutine(ExpressionCoroutine(blendShapeName, weight, speedMultiplier, immediate));
            expressionCoroutines[blendShapeName] = expressionCoroutine;
        }

        private IEnumerator ExpressionCoroutine(string blendShapeName, float weight, float speedMultiplier, bool immediate)
        {
            int blendShapeIndex = modelExpressionController.sharedMesh.GetBlendShapeIndex(blendShapeName);
            if (blendShapeIndex == -1)
            {
                Debug.Log($"Character {name} doesnt have a blend shape {blendShapeName}");
                yield break;
            }

            if (immediate)
            {
                modelExpressionController.SetBlendShapeWeight(blendShapeIndex, weight);
            }
            else
            {
                float currentValue = modelExpressionController.GetBlendShapeWeight(blendShapeIndex);
                while (currentValue != weight)
                {
                    currentValue = Mathf.MoveTowards(currentValue, weight, speedMultiplier * Time.deltaTime * expression_transition_speed);
                    modelExpressionController.SetBlendShapeWeight(blendShapeIndex, currentValue);
                    yield return null;
                }
            }

            expressionCoroutines.Remove(blendShapeName);
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

        public override void SetColor (Color color)
        {
            base.SetColor (color);

            renderer.color = color;
            foreach (var or in oldRenderers)
                or.oldImage.color = color;
        }

        public override IEnumerator ChangingColor(float speedMultiplier)
        {

            yield return ChangingRendererColor(speedMultiplier);
            co_changingColor = null;
        }

        public override IEnumerator Highlighting(float speedMultiplier)
        {
            if (!isChangingColor)
                yield return ChangingRendererColor(speedMultiplier);

            co_highlighting = null;
        }

        private IEnumerator ChangingRendererColor(float speedMultiplier)
        {
            Color oldColor = renderer.color;

            float colorPercent = 0;
            while (colorPercent != 1)
            {
                colorPercent += default_transition_speed * speedMultiplier * Time.deltaTime;

                renderer.color = Color.Lerp(oldColor, displayColor, colorPercent);

                foreach (var or in oldRenderers)
                    or.oldImage.color = renderer.color;

                yield return null;
            }

            co_changingColor = null;
        }

        private void CreateNewCharacterRenderingInstance()
        {
            oldRenderers.Add(new OldRenderer(rendererCG, renderer, renderGroup));
            renderGroup = Object.Instantiate(renderGroup, renderGroup.transform.parent);
            renderGroup.name = string.Format(character_render_group_prefab_name_format, name);
            camera = renderGroup.GetComponentInChildren<Camera>();
            modelContainer = camera.transform.GetChild(0);
            model = modelContainer.GetChild(0);
            modelAnimator = model.GetComponent<Animator>();
            modelExpressionController = model.GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(sm => sm.sharedMesh.blendShapeCount > 0);

            string rendererName = renderer.name;
            Texture oldRenderTexture = renderer.texture;
            renderer = Object.Instantiate(renderer.gameObject, renderer.transform.parent).GetComponent<RawImage>();
            renderer.name = rendererName;
            rendererCG.alpha = 0;
            RenderTexture newTex = new RenderTexture(oldRenderTexture as RenderTexture);
            renderer.texture = newTex;
            camera.targetTexture = newTex;

            for (int i = 0; i < oldRenderers.Count; i++)
                oldRenderers[i].oldRenderGroup.transform.localPosition = Vector3.zero + (Vector3.right * i);

            renderGroup.transform.position = Vector2.zero + (Vector2.right * (character_stacking_depth + oldRenderers.Count));
        }

        private IEnumerator FadingOutOldRenderers()
        {
            while(oldRenderers.Any(o => o.oldCG.alpha > 0))
            {
                float speed = default_transition_speed * Time.deltaTime * oldRendererFadeOutSpeedMultiplier;
                foreach (var or in oldRenderers)
                    or.oldCG.alpha = Mathf.MoveTowards(or.oldCG.alpha, 0, speed);

                yield return null;
            }

            foreach (var or in oldRenderers)
            {
                Object.Destroy(or.oldRenderGroup);
                Object.Destroy(or.oldCG.gameObject);
            }

            oldRenderers.Clear();

            co_fadingOutOldRenderers = null;
        }
        public override IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            Vector3 facingAngle = new Vector3(0, (facingLeft ? default_facing_direction_value : -default_facing_direction_value), 0);
            if (immediate)
            {
                modelContainer.localEulerAngles = facingAngle;
            }
            else
            {
                CreateNewCharacterRenderingInstance();
                modelContainer.localEulerAngles = facingAngle;

                oldRendererFadeOutSpeedMultiplier = speedMultiplier;
                if (!isFadingOutOldRenderers)
                    co_fadingOutOldRenderers = characterManager.StartCoroutine(FadingOutOldRenderers());

                CanvasGroup newRenderer = rendererCG;
                while(newRenderer.alpha != 1)
                {
                    float speed = default_transition_speed * Time.deltaTime * speedMultiplier;
                    newRenderer.alpha = Mathf.MoveTowards(newRenderer.alpha, 1, speed);
                    yield return null;
                }
            }
            co_flipping = null;
        }
    }
}