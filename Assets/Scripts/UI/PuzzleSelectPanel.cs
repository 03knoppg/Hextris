using System.Collections.Generic;
using UnityEngine;

public class PuzzleSelectPanel : UIThing {

    public PuzzleSelectButton PuzzleSelectButtonPrefab;

    int offset = 0;
    public int buttonsPerPage = 8;

    List<PuzzleSelectButton> buttons = new List<PuzzleSelectButton>();

    public UIButton nextButton;
    public UIButton prevButton;

    List<Puzzle> puzzles;

    protected override void OnStateChanged(UIStates.State newState)
    {
        switch (newState)
        {
            case UIStates.State.Active:
                RepopulateButtons();
                break;
        }

        base.OnStateChanged(newState);
    }

    void RepopulateButtons()
    {
        puzzles = Progression.Puzzles.Obj;

        foreach (PuzzleSelectButton button in buttons)
            Destroy(button.gameObject);

        buttons.Clear();

        for (int i = buttonsPerPage * offset; i < Mathf.Min(puzzles.Count, buttonsPerPage * (offset + 1)); i++)
        {
            PuzzleSelectButton button = Instantiate(PuzzleSelectButtonPrefab);
            button.levelIndex = i;
            button.SetText(puzzles[i].name);
            button.transform.SetParent(transform);
            buttons.Add(button);
        }

        prevButton.State = offset > 0 ? UIStates.State.Active : UIStates.State.Disabled;
        nextButton.State = offset < Mathf.Floor(puzzles.Count / buttonsPerPage) ? UIStates.State.Active : UIStates.State.Disabled;
    }

    public void UnlockAll()
    {
        Progression.UnlockAll();
    }

    public void NextPage()
    {
        offset = Mathf.Min(offset + 1, Mathf.FloorToInt(puzzles.Count / buttonsPerPage));
        RepopulateButtons();
    }

    public void PrevPage()
    {
        offset = Mathf.Max(offset - 1, 0);
        RepopulateButtons();
    }
}
