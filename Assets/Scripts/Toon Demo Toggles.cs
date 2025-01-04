using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class ToonDemoToogles : MonoBehaviour
{
    private const string BLINN_PHONG_PROPERTY = "_BLINN_PHONG";
	[SerializeField] private Material[] _toonMaterials;
	private LocalKeyword _blinnPhongKeyword;

	private void Awake()
	{
		_blinnPhongKeyword = _toonMaterials.First().shader.keywordSpace.FindKeyword(BLINN_PHONG_PROPERTY);
	}

	public void SetBlinnPhong(bool value) => Array.ForEach(_toonMaterials, m => m.SetKeyword(_blinnPhongKeyword, value));
}
