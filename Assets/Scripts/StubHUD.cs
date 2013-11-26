using UnityEngine;
using System.Collections;

public class StubHUD : MonoBehaviour
{
	public Player player;
	public GUIText finalDistanceLabelText;
	public GUIText finalDistanceText;
	public GUIText crystalsCollectedLabelText;
	public GUIText crystalsCollectedText;
	public GUIText moneyText;
	public GUIText distanceText;
	public GUIText debugText;
	
	public GUIStyle areaStyle;
	public GUIStyle redButtonStyle;
	public GUIStyle blueButtonStyle;
	public GUIStyle greenButtonStyle;
	public GUIStyle greyButtonStyle;
	
	const float AREA_WIDTH = 400.0f;
	const float AREA_HEIGHT = 350.0f;
	
	Treadmill treadmill;

	void Awake ()
	{
		SetItemTexts ();
		treadmill = GameObject.Find (ObjectNames.TREADMILL).GetComponent<Treadmill> ();
	}
	
	void Update ()
	{

	}
	
	void OnGUI ()
	{
		if (GameManager.Instance.IsOnMenu ()) {
			DisplayMainMenu ();
		} else if (GameManager.Instance.IsGameOver ()) {
			DisplayDeadMenu ();
		} else if (GameManager.Instance.IsShopping ()) {
			DisplayStoreMenu ();
		} else {
			DisplayInGameText ();
		}
	}
	
	/*
	 * Display the distance and any other in game text we need to show the player.
	 */
	void DisplayInGameText ()
	{
		EnableInGameText (true);
		EnableGameOverText (false);
		distanceText.text = "Distance: " + Mathf.RoundToInt (treadmill.distanceTraveled);
		PrintMoneyToScreen ();
		debugText.text = string.Format ("Passed Pigments: {0}\nHealth: {1}\nWildcards: {2}\nDifficulty: {3}",
			GameManager.Instance.numPickupsPassed, player.curHealth, player.WildcardCount,
			GameManager.Instance.difficulty);
	}
	
	/*
	 * Display the menu for when the player is dead. Receive inputs and call
	 * the appropriate GameManager implemented method.
	 */
	void DisplayDeadMenu ()
	{
		EnableInGameText (false);
		EnableGameOverText (true);
		finalDistanceLabelText.text = ("You had\n a colorful run of");
		finalDistanceText.text = Mathf.RoundToInt (treadmill.distanceTraveled) + "m";
		crystalsCollectedLabelText.text = "Crystals Collected";
		crystalsCollectedText.text = GameManager.Instance.numPointsThisRound.ToString ();
		GUILayout.BeginArea (new Rect ((Screen.width - AREA_WIDTH)/2, (Screen.height - AREA_HEIGHT), AREA_WIDTH, AREA_HEIGHT), areaStyle);
		// Push our buttons down to bottom of screen
		GUILayout.BeginVertical ();
		GUILayout.FlexibleSpace ();
		// Center our buttons with space on both sides
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("Go to Store", greenButtonStyle)) {
			GameManager.Instance.GoToStore ();
		}
		if (GUILayout.Button ("Retry [Enter]", blueButtonStyle)) {
			//Application.LoadLevel (Application.loadedLevel);
			GameManager.Instance.StartGame (false);
			EnableInGameText (true);
		}
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
	}
	
	/*
	 * Helper method to display money on GUIText object.
	 */
	void PrintMoneyToScreen ()
	{
		moneyText.enabled = true;
		moneyText.text = "Money: " + player.money;
	}
	
	/*
	 * Display the menu for when the player is at the store. Receive inputs and call
	 * the appropriate GameManager implemented methods.
	 */
	void DisplayStoreMenu ()
	{
		EnableGameOverText (false);
		EnableInGameText (false);
		PrintMoneyToScreen ();
		Store store = (Store)GameObject.Find (ObjectNames.STORE).GetComponent<Store> ();
		
		// TODO Let's at least make the Buy/AlreadyOwned a 3d button on the item mesh
		GUILayout.BeginArea (new Rect (Screen.width - AREA_WIDTH, (Screen.height - AREA_HEIGHT), AREA_WIDTH, AREA_HEIGHT), areaStyle);
		// Push our buttons to bottom of screen
		GUILayout.BeginVertical ();
		GUILayout.FlexibleSpace ();
		// Push our buttons to right of screen.
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		if (store.IsAlreadyPurchased ()) {
			GUILayout.Button ("Already Owned", greyButtonStyle);
		}
		else if (!store.HasEnoughMoney ()) {
			GUILayout.Button ("Not Enough Money", redButtonStyle);
		} else {
			if (GUILayout.Button ("Buy (" + store.GetSelectedItem ().cost + ")", greenButtonStyle)) {
				store.BuyItem ();
			}
		}
		GUILayout.EndHorizontal ();
		// Push more buttons to the right of the screen
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("Play", blueButtonStyle)) {
			GameManager.Instance.GoToGame ();
			GameManager.Instance.StartGame (true);
		}
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
	}
	
	void SetItemTexts ()
	{
		GameObject[] itemObjs = GameObject.FindGameObjectsWithTag (Tags.ITEM);
		foreach (GameObject obj in itemObjs) {
			Item item = obj.GetComponent<Item> ();
			TextMesh itemText = obj.GetComponentInChildren<TextMesh> ();
			itemText.text = string.Format ("{0}\n\nCost: {1}", item.itemName, item.cost);
		}
	}
	
	/*
	 * Show the buttons for the main menu and include logic when buttons
	 * are pressed.
	 */
	void DisplayMainMenu ()
	{
		EnableInGameText (false);
		EnableGameOverText (false);
		GUILayout.BeginArea (new Rect ((Screen.width - AREA_WIDTH)/2, (Screen.height - AREA_HEIGHT), AREA_WIDTH, AREA_HEIGHT), areaStyle);
		// Push buttons to the bottom of the screen
		GUILayout.BeginVertical ();
		GUILayout.FlexibleSpace ();
		GUILayout.BeginHorizontal ();
		// Push buttons to the center of the screen
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button ("Go to Store", greenButtonStyle)) {
			GameManager.Instance.GoToStore ();
		}
		if (GUILayout.Button ("Start Game [ENTER]", blueButtonStyle)) {
			GameManager.Instance.GoToGame ();
			GameManager.Instance.StartGame (true);
		}
		// Push more buttons to the center of the screen
		GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		GUILayout.EndVertical ();
		GUILayout.EndArea ();
	}
	
	/*
	 * Helper method to enable or disable in game text.
	 */
	void EnableInGameText (bool enable)
	{
		moneyText.enabled = enable;
		distanceText.enabled = enable;
		debugText.enabled = enable;
	}
	
	/*
	 * Helper method to enable or disable game over text elements.
	 */
	void EnableGameOverText (bool enable)
	{
		finalDistanceLabelText.enabled = enable;
		finalDistanceText.enabled = enable;
		crystalsCollectedText.enabled = enable;
		crystalsCollectedLabelText.enabled = enable;
	}

}
