using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace Visuals
{
	public class NightVisionController : MonoBehaviour
	{
		[SerializeField] private ScriptableRendererFeature _rendererFeature;

		private void Update()
		{
			if (Mouse.current.rightButton.wasPressedThisFrame)
				_rendererFeature.SetActive(!_rendererFeature.isActive);
		}
	}
}
