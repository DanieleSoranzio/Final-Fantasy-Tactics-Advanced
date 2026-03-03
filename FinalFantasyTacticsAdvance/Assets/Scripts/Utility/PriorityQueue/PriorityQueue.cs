using UnityEngine;


public class PriorityQueue<T>
{
    #region Data
    const int baseHeapSize = 100;

    PriorityNode<T>[] heap;
    int count;


    public int Count => count;
    #endregion


    #region Methods
    public PriorityQueue()
    {
        heap = new PriorityNode<T>[baseHeapSize];
        count = 0;
    }


    public void Enqueue(T data, float priority, float heuristic = 0)
    {
        PriorityNode<T> node = new PriorityNode<T>(data, priority, heuristic);

        if (count == heap.Length)
            ResizeQueue();

        heap[count] = node;
        count++;

        SiftUp(count - 1);
    }


    public T Dequeue()
    {
        PriorityNode<T> node;

        if (count == 0)
            throw new System.Exception("Priority Queue is currently empty!");
        else
        {
            node = heap[0];
            heap[0] = heap[count - 1];
            count--;

            if (count > 0)
                SiftDown(0);
        }

        heap[count] = null;
        return node.Data;
    }


    public void PrintQueue()
    {
        string print = "";

        for (int i = 0; i < count; i++)
            print += $"#{i} {heap[i]}\n";

        Debug.Log(print);
    }


    private void ResizeQueue()
    {
        PriorityNode<T>[] newHeap = new PriorityNode<T>[2 * count];

        for (int i = 0; i < heap.Length; i++)
            newHeap[i] = heap[i];

        heap = newHeap;
    }


    private void SiftUp(int index)
    {
        if (index != 0)
        {
            int parentIndex = GetParentIndex(index);
            if (heap[parentIndex] > heap[index])
            {
                PriorityNode<T> tmp = heap[parentIndex];
                heap[parentIndex] = heap[index];
                heap[index] = tmp;

                SiftUp(parentIndex);
            }
        }
    }


    private void SiftDown(int index)
    {
        int leftIndex, rightIndex, minIndex;
        PriorityNode<T> tmp;

        leftIndex = GetLeftChildIndex(index);
        rightIndex = GetRightChildIndex(index);

        if (rightIndex >= count)
        {
            if (leftIndex >= count)
                return;
            else
                minIndex = leftIndex;
        }
        else
        {
            if (heap[leftIndex] <= heap[rightIndex])
                minIndex = leftIndex;
            else
                minIndex = rightIndex;
        }

        if (heap[index] > heap[minIndex])
        {
            tmp = heap[minIndex];
            heap[minIndex] = heap[index];
            heap[index] = tmp;

            SiftDown(minIndex);
        }
    }


    private int GetParentIndex(int index)
    {
        return (index - 1) / 2;
    }


    private int GetLeftChildIndex(int index)
    {
        return (2 * index) + 1;
    }


    private int GetRightChildIndex(int index)
    {
        return (2 * index) + 2;
    }
    #endregion
}
