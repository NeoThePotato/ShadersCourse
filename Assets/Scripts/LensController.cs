using UnityEngine;
using UnityEngine.InputSystem;

namespace Visuals
{
	public class LensController : MonoBehaviour
	{
		const string SIZE_PROPERTY = "_Size";
		const string CENTER_PROPERTY = "_Center";

		[SerializeField] private FullScreenPassRendererFeature _rendererFeature;
		[SerializeField] private float _scrollSensitivity = 2e-4f;
		private Material _material;
		private Camera _camera;
		private float _size = 0.1f;
		private float Size { get => _size; set => _size = Mathf.Clamp01(value); }

		private void OnEnable()
		{
			_camera = Camera.main;
			_material = _rendererFeature.passMaterial;
			_material.hideFlags = HideFlags.DontSave;
			_rendererFeature.SetActive(true);
			Update();
		}

		private void OnDisable()
		{
			_rendererFeature.SetActive(false);
		}

		private void Update()
		{
			Size += Mouse.current.scroll.ReadValue().y * _scrollSensitivity;
			_material.SetFloat(SIZE_PROPERTY, Size);
			Vector2 dimensions = new(_camera.pixelWidth, _camera.pixelHeight);
			_material.SetVector(CENTER_PROPERTY, Mouse.current.position.ReadValue() / dimensions);
		}
	}
}
