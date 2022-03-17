using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDragSlot
{
    void OnDragBegin(DragHandle handle);
    void OnDragEnd(DragHandle handle);
}
