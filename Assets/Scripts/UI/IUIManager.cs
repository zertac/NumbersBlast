namespace NumbersBlast.UI
{
    public interface IUIManager
    {
        T ShowPopup<T>() where T : BasePopup;
        void CloseCurrentPopup();
        T GetPopup<T>() where T : BasePopup;
        bool IsPopupActive { get; }
    }
}
