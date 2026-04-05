using UnityEngine;

public class TutorialManager
{
    private const string TutorialCompleteKey = "TutorialComplete";

    private readonly TutorialConfig _config;
    private readonly BoardManager _boardManager;
    private readonly BoardView _boardView;
    private readonly PieceTray _pieceTray;
    private readonly TutorialOverlay _overlay;
    private readonly TutorialFeedbackPopup _feedbackPopup;
    private readonly BoardConfig _boardConfig;

    private int _currentStepIndex;
    private bool _isActive;
    private TutorialStepData _currentStep;

    public bool IsActive => _isActive;
    public Vector2Int? ForcedPlacement => _isActive ? _currentStep?.TargetBoardPosition : null;

    public TutorialManager(TutorialConfig config, BoardManager boardManager, BoardView boardView,
        PieceTray pieceTray, TutorialOverlay overlay, TutorialFeedbackPopup feedbackPopup, BoardConfig boardConfig)
    {
        _config = config;
        _boardManager = boardManager;
        _boardView = boardView;
        _pieceTray = pieceTray;
        _overlay = overlay;
        _feedbackPopup = feedbackPopup;
        _boardConfig = boardConfig;
    }

    public bool ShouldRunTutorial()
    {
        return _config != null && _config.Steps != null && _config.Steps.Length > 0
            && PlayerPrefs.GetInt(TutorialCompleteKey, 0) == 0;
    }

    public void StartTutorial()
    {
        _isActive = true;
        _currentStepIndex = 0;
        _overlay.Initialize();
        _feedbackPopup.Initialize();
        GameEvents.OnPiecePlaced += HandlePiecePlaced;
        LoadStep(_currentStepIndex);
    }

    public void StopTutorial()
    {
        _isActive = false;
        GameEvents.OnPiecePlaced -= HandlePiecePlaced;
        _overlay.Hide();
        ClearBoardHighlight();
        PlayerPrefs.SetInt(TutorialCompleteKey, 1);
        PlayerPrefs.Save();
    }

    private void LoadStep(int index)
    {
        _currentStep = _config.Steps[index];

        SetupBoard();
        SpawnTutorialPiece();

        // Force layout rebuild so positions are correct
        Canvas.ForceUpdateCanvases();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(
            _pieceTray.GetComponent<RectTransform>());

        _overlay.Show();
        _overlay.SetInstruction(_currentStep.InstructionText);

        // Highlight target cells on board + piece in tray
        HighlightTargetCells();

        var pieceRect = _pieceTray.GetFirstOccupiedPieceRect();
        if (pieceRect != null)
            _overlay.HighlightRect(pieceRect);
    }

    private void SetupBoard()
    {
        var model = _boardManager.Model;

        for (int r = 0; r < model.Rows; r++)
        {
            for (int c = 0; c < model.Columns; c++)
            {
                int value = 0;
                if (r < _currentStep.BoardRows && c < _currentStep.BoardColumns)
                    value = _currentStep.GetBoardValue(r, c);

                if (value > 0)
                    model.GetCell(r, c).SetValue(value);
                else
                    model.GetCell(r, c).Clear();
            }
        }

        _boardView.RefreshAll();
    }

    private void SpawnTutorialPiece()
    {
        var positions = _currentStep.GetPieceNormalizedPositions();
        var values = _currentStep.GetPieceFixedValues();
        var pieceModel = new PieceModel(positions, values);

        _pieceTray.SpawnTutorialPiece(pieceModel);
    }

    private void HighlightTargetCells()
    {
        var target = _currentStep.TargetBoardPosition;
        var positions = _currentStep.GetPieceNormalizedPositions();

        int minRow = int.MaxValue, maxRow = 0, minCol = int.MaxValue, maxCol = 0;

        for (int i = 0; i < positions.Length; i++)
        {
            int r = target.x + positions[i].x;
            int c = target.y + positions[i].y;

            if (r < minRow) minRow = r;
            if (r > maxRow) maxRow = r;
            if (c < minCol) minCol = c;
            if (c > maxCol) maxCol = c;

            var cellView = _boardView.GetCellView(r, c);
            cellView?.SetHighlight(HighlightType.TutorialTarget);
        }

        var topLeft = _boardView.GetCellView(minRow, minCol);
        var bottomRight = _boardView.GetCellView(maxRow, maxCol);
        if (topLeft != null && bottomRight != null)
        {
            _overlay.HighlightTwoCells(topLeft, bottomRight);
        }
    }

    private void ClearBoardHighlight()
    {
        var cellViews = _boardView.CellViews;
        if (cellViews == null) return;

        for (int r = 0; r < cellViews.GetLength(0); r++)
            for (int c = 0; c < cellViews.GetLength(1); c++)
                cellViews[r, c].SetHighlight(HighlightType.None);
    }

    private void HandlePiecePlaced(PieceView pieceView, Vector2Int boardPos)
    {
        if (!_isActive) return;

        ClearBoardHighlight();
        _overlay.HideHand();

        string title;
        string desc;

        switch (_currentStepIndex)
        {
            case 0:
                title = "Well Done!";
                desc = "You placed your first block!";
                break;
            case 1:
                title = "Great!";
                desc = "Same numbers merge together!";
                break;
            case 2:
                title = "Awesome!";
                desc = "Full rows and columns get cleared for points!";
                break;
            default:
                title = "Nice!";
                desc = "";
                break;
        }

        _overlay.Hide();

        _feedbackPopup.Show(title, desc, () =>
        {
            _currentStepIndex++;

            if (_currentStepIndex >= _config.Steps.Length)
            {
                StopTutorial();
                StartNormalGame();
            }
            else
            {
                LoadStep(_currentStepIndex);
            }
        });
    }

    public void OnPiecePickedUp()
    {
        if (!_isActive) return;

        Canvas.ForceUpdateCanvases();
        HighlightTargetCells();
        _overlay.SetInstruction("Place it here!");
    }

    private void StartNormalGame()
    {
        var model = _boardManager.Model;
        for (int r = 0; r < model.Rows; r++)
            for (int c = 0; c < model.Columns; c++)
                model.GetCell(r, c).Clear();

        _boardView.RefreshAll();
        _pieceTray.ClearAll();
        _pieceTray.SpawnPieces();
    }
}
