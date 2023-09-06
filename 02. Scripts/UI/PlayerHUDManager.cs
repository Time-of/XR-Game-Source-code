using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;



/**
 * @author 이성수
 * @brief 플레이어의 HUD를 컨트롤하는 역할의 클래스입니다.
 * @since 2023-03-25
 */
public class PlayerHUDManager : MonoBehaviour
{
	[SerializeField]
	private GameObject CurrentActivatedPanel;

	[SerializeField]
	private Canvas MenuCanvas;

	[SerializeField]
	private TMP_Text MoneyText;

	private bool bIsMenuCanvasShowing = false;

	public WeaponInventory WeaponInventoryUI;

	[SerializeField]
	private GameObject DefeatTextObjectOnActivatedOnDefeat;

	[SerializeField]
	private TMP_Text ScoreTextActivatedOnDefeat;

	[SerializeField]
	private GameObject VictoryTextObjectOnActivatedOnVictory;

	[SerializeField]
	private TMP_Text ScoreTextActivatedOnVictory;



	private void Start()
	{
		GameManager.instance.OnMoneyUpdatedEvent.AddListener(SetMoneyText);
		GameManager.instance.OnDefeatEvent.AddListener(OnDefeat);
		GameManager.instance.OnVictoryEvent.AddListener(OnVictory);
	}



	public void QuitGame()
	{
		Application.Quit();
	}



	private void OnDefeat(uint Score)
	{
		DefeatTextObjectOnActivatedOnDefeat.gameObject.SetActive(true);
		ScoreTextActivatedOnDefeat.gameObject.SetActive(true);
		ScoreTextActivatedOnDefeat.text = "점수: " + Score;
	}



	private void OnVictory(uint Score)
	{
		VictoryTextObjectOnActivatedOnVictory.gameObject.SetActive(true);
		ScoreTextActivatedOnVictory.gameObject.SetActive(true);
		ScoreTextActivatedOnVictory.text = "점수: " + Score;
	}



	public void ChangePanel(GameObject NewPanel)
	{
		if (CurrentActivatedPanel != null) CurrentActivatedPanel.SetActive(false);

		CurrentActivatedPanel = NewPanel;
		CurrentActivatedPanel.SetActive(true);
	}



	public void ToggleMenuUI()
	{
		bIsMenuCanvasShowing = !bIsMenuCanvasShowing;
		MenuCanvas.gameObject.SetActive(bIsMenuCanvasShowing);

		SetMoneyText(GameManager.instance.GetMoney());
	}



	public void ForceCloseMenuUI()
	{
		bIsMenuCanvasShowing = false;
		MenuCanvas.gameObject.SetActive(bIsMenuCanvasShowing);
	}



	public void SetMoneyText(uint MoneyAmount)
	{
		MoneyText.text = MoneyAmount.ToString();
	}
}
