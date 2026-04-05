#if UNITY_EDITOR
using UnityEngine;

[CreateAssetMenu(fileName = "DevUISprites", menuName = "NumbersBlast/Dev/UI Sprites")]
public class DevUISprites : ScriptableObject
{
    [Header("Main Menu Buttons")]
    public Sprite PlayButtonSprite;
    public Sprite MultiplayerButtonSprite;
    public Sprite SettingsButtonSprite;
    public Sprite ExitButtonSprite;

    [Header("Gameplay HUD")]
    public Sprite PauseButtonSprite;
    public Sprite GameSettingsButtonSprite;
    public Sprite ScoreBackgroundSprite;

    [Header("Popup Buttons")]
    public Sprite ResumeButtonSprite;
    public Sprite RestartButtonSprite;
    public Sprite MainMenuButtonSprite;
    public Sprite ContinueButtonSprite;
    public Sprite CloseButtonSprite;
    public Sprite CancelButtonSprite;

    [Header("Popup Backgrounds")]
    public Sprite PopupBackgroundSprite;
    public Sprite DimBackgroundSprite;

    [Header("Toggle Buttons")]
    public Sprite ToggleOnSprite;
    public Sprite ToggleOffSprite;

    [Header("Multiplayer HUD")]
    public Sprite TimerBarSprite;
    public Sprite TimerBarBackgroundSprite;
    public Sprite PlayerScoreBackgroundSprite;
    public Sprite OpponentScoreBackgroundSprite;

    [Header("General")]
    public Sprite BackgroundSprite;
}
#endif
