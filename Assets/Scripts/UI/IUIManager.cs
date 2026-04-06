namespace NumbersBlast.UI
{
    /// <summary>
    /// Abstraction for the UI popup management system, providing methods to show, close, and query popups.
    /// </summary>
    public interface IUIManager
    {
        /// <summary>
        /// Shows a popup of the specified type, closing any currently active popup first.
        /// </summary>
        T ShowPopup<T>() where T : BasePopup;

        /// <summary>
        /// Closes the currently displayed popup, if any.
        /// </summary>
        void CloseCurrentPopup();

        /// <summary>
        /// Returns a cached popup instance of the specified type, or null if it has not been created yet.
        /// </summary>
        T GetPopup<T>() where T : BasePopup;

        /// <summary>
        /// Gets whether a popup is currently active and visible.
        /// </summary>
        bool IsPopupActive { get; }
    }
}
