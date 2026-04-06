namespace NumbersBlast.Board
{
    /// <summary>
    /// Defines the visual highlight states that can be applied to board cells.
    /// </summary>
    public enum HighlightType
    {
        /// <summary>No highlight applied.</summary>
        None,
        /// <summary>Valid placement preview.</summary>
        Placement,
        /// <summary>Invalid placement indicator.</summary>
        Invalid,
        /// <summary>Adjacent same-value merge candidate.</summary>
        Merge,
        /// <summary>Full row or column about to be cleared.</summary>
        LineClear,
        /// <summary>Tutorial target cell highlight.</summary>
        TutorialTarget
    }
}
