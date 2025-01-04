using DG.Tweening;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class ToonDemoToogles : MonoBehaviour
{
    private const string BLINN_PHONG_PROPERTY = "_BLINN_PHONG";
	[SerializeField] private Material[] _toonMaterials;
	[SerializeField] private string _rotateID, _colorID;
	private LocalKeyword _blinnPhongKeyword;

	private void Awake()
	{
		_blinnPhongKeyword = _toonMaterials.First().shader.keywordSpace.FindKeyword(BLINN_PHONG_PROPERTY);
	}

	public void SetBlinnPhong(bool value) => Array.ForEach(_toonMaterials, m => m.SetKeyword(_blinnPhongKeyword, value));

	public void SetRotationAnimation(bool value) => SetDOTweenPlay(_rotateID, value);

	public void SetColorAnimation(bool value) => SetDOTweenPlay(_colorID, value);

	private static void SetDOTweenPlay(string id, bool value)
	{
		if (value)
			DOTween.Play(id);
		else
			DOTween.Pause(id);
	}
}
