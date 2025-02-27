using UnityEngine;
using UnityEngine.UI;

public class MorphButton : MonoBehaviour
{
	[SerializeField] private Button _button;
	[SerializeField] private MorphController _morphController;

	public void OnClick() => _button.interactable = !_morphController.TryMorph(Callback);

	private void Callback() => _button.interactable = true;
}
