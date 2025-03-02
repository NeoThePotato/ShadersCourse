using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using DG.Tweening;
using FloatTweener = DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions>;

public class MorphController : MonoBehaviour
{
	private const string	PROGRESS_PROPERTY = "_Progress",
							SIZE_PROPERTY = "_Size",
							LIFETIME_PROPERTY = "Lifetime",
							START_PROPERTY = "Start",
							END_PROPERTY = "End",
							PLAY_EVENT = "OnPlay",
							STOP_EVENT = "OnStop";

	[SerializeField] private GameObject _start, _end;
	[SerializeField] private VisualEffect _vfxPrefab;
	[SerializeField] private Material _dissolveMaterial;
	[SerializeField] private Shader _originalShader;
	[SerializeField] private float _animationDuration = 2f;
	private HashSet<Material> _materials;
	private Sequence _animation;

	private Shader DissolveShader => _dissolveMaterial.shader;

	private void OnValidate()
	{
		_end.SetActive(false);
		_originalShader = _start.GetComponentInChildren<Renderer>().sharedMaterial.shader;
	}

	private void Awake() => _materials = new();

	private void OnDisable() => RestoreOriginalShader();

	public bool TryMorph(UnityAction onComplete)
	{
		if (_animation == null || !_animation.active)
		{
			Morph(onComplete);
			return true;
		}
		return false;
	}

	private void Morph(UnityAction onComplete)
	{
		GetDissolveAndAppear(out var dissolveGO, out var appearGO);
		var dissolveMaterials = GetUniqueMaterials(dissolveGO);
		var appearMaterials = GetUniqueMaterials(appearGO);
		float delay = _vfxPrefab.GetFloat(LIFETIME_PROPERTY);
		float fullProgress = _dissolveMaterial.GetFloat(SIZE_PROPERTY) + 1f;
		_materials.UnionWith(dissolveMaterials.Union(appearMaterials));
		SetShader(dissolveMaterials, DissolveShader);
		SetShader(appearMaterials, DissolveShader);
		SetProgressProperty(dissolveMaterials, 0f);
		SetProgressProperty(appearMaterials, fullProgress);
		appearGO.SetActive(true);
		var VFXes = SpawnVFXes().ToArray();
		foreach (var vfx in VFXes)
			vfx.SendEvent(PLAY_EVENT);
		PlayAnimation();

		void PlayAnimation()
		{
			_animation = DOTween.Sequence(this);
			Dissolve(_animation, fullProgress);
			Appear(_animation, delay);
			foreach (var vfx in VFXes)
				_animation.InsertCallback(_animationDuration, () => OnVFXFinish(vfx));
			_animation.OnComplete(OnComplete);

			void Dissolve(Sequence sequence, float fullProgress)
			{
				foreach (var mat in dissolveMaterials)
					sequence.Join(TweenProgress(mat, fullProgress));
			}

			void Appear(Sequence sequence, float delay)
			{
				sequence.Insert(delay, TweenProgress(appearMaterials.First(), 0f));
				foreach (var mat in appearMaterials.Skip(1))
					sequence.Join(TweenProgress(mat, 0f));
			}

			FloatTweener TweenProgress(Material material, float endValue)
			{
				return material.DOFloat(endValue, PROGRESS_PROPERTY, _animationDuration).SetEase(Ease.InOutSine);
			}

			void OnComplete()
			{
				dissolveGO.SetActive(false);
				RestoreOriginalShader();
				onComplete?.Invoke();
				foreach (var vfx in VFXes)
					vfx.gameObject.SetActive(false);
			}

			void OnVFXFinish(VisualEffect vfx)
			{
				vfx.SendEvent(STOP_EVENT);
			}
		}

		void SetProgressProperty(IEnumerable<Material> materials, float value)
		{
			foreach (var material in materials)
				material.SetFloat(PROGRESS_PROPERTY, value);
		}

		IEnumerable<VisualEffect> SpawnVFXes()
		{
			foreach ((var start, var end) in dissolveGO.GetComponentsInChildren<SkinnedMeshRenderer>().Zip(appearGO.GetComponentsInChildren<SkinnedMeshRenderer>(), (s, e) => (s, e)))
				yield return SpawnVFX(start, end);

			VisualEffect SpawnVFX(SkinnedMeshRenderer start, SkinnedMeshRenderer end)
			{
				var vfx = GetOrCreateVFX();
				vfx.SetSkinnedMeshRenderer(START_PROPERTY, start);
				vfx.SetSkinnedMeshRenderer(END_PROPERTY, end);
				return vfx;
			}

			VisualEffect GetOrCreateVFX()
			{
				var vfx = GetComponentsInChildren<VisualEffect>(includeInactive: true).FirstOrDefault(vfx => !vfx.gameObject.activeSelf);
				if (vfx == null)
					vfx = Instantiate(_vfxPrefab, transform);
				vfx.gameObject.SetActive(true);
				return vfx;
			}
		}

		void GetDissolveAndAppear(out GameObject dissolve, out GameObject appear)
		{
			dissolve = _start;
			appear = _end;
			if (!_start.activeSelf)
				(dissolve, appear) = (appear, dissolve);
		}

		IEnumerable<Material> GetUniqueMaterials(GameObject go) => go.GetComponentsInChildren<Renderer>().Select(r => r.sharedMaterial).Distinct();
	}

	private void SetShader(IEnumerable<Material> materials, Shader shader)
	{
		foreach (var m in materials)
			m.shader = shader;
	}

	private void RestoreOriginalShader() => SetShader(_materials, _originalShader);
}
