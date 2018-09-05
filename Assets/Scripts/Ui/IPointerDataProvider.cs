using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects;
using UnityEngine;

namespace Assets.Scripts.Ui
{
    public interface IPointerDataProvider
    {
        TileObject UnderCursorTileObject { get; }

        UiElement UnderCursorUiElement { get; }

        Vector2Int UnderCursorCell { get; }

        Vector2 MouseWorldPosition { get; }
        Vector2 MouseScreenPosition { get; }

        bool PrevLmbState { get; }
        bool CurrentLmbState { get; }

    }
}
