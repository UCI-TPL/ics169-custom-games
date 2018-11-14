using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonCellPQ{

    int count = 0;

    int minimum = int.MaxValue;

    List<HexagonCell> list = new List<HexagonCell>();


    public int Count
    {
        get
        {
            return count;
        }
    }

    public void Enqueue(HexagonCell cell)
    {
        count += 1;
        int priority = cell.SearchPriority;
        while (priority >= list.Count)
            list.Add(null);
        cell.NextWithSamePriority = list[priority];
        list[priority] = cell;
    }

	public HexagonCell Dequeue()
    {
        count -= 1;
        for(; minimum < list.Count; minimum++)
        {
            HexagonCell cell = list[minimum];
            if (cell != null)
            {
                list[minimum] = cell.NextWithSamePriority;
                return cell;
            }
        }
        return null;
    }

    public void Change(HexagonCell cell, int oldPriority)
    {
        HexagonCell current = list[oldPriority];
        HexagonCell next = current.NextWithSamePriority;
        if (current == cell)
            list[oldPriority] = next;
        else
        {
            while(next != cell)
            {
                current = next;
                next = current.NextWithSamePriority;
            }
            current.NextWithSamePriority = cell.NextWithSamePriority;
        }
        Enqueue(cell);
        count -= 1;
    }

    public void Clear()
    {
        list.Clear();
        count = 0;
        minimum = int.MaxValue;
    }
}
