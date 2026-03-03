using System;


public class PriorityNode<T>
{
    #region Data
    T data;
    float priority;
    float heuristic;


    public T Data => data;
    public float Priority => priority;
    public float Heuristic => heuristic;
    #endregion


    #region Methods
    public PriorityNode(T data, float priority, float heuristic)
    {
        this.data = data;
        this.priority = priority;
        this.heuristic = heuristic;
    }


    public static bool operator <(PriorityNode<T> a, PriorityNode<T> b) => a.priority < b.priority ? true : a.priority > b.priority ? false : a.heuristic < b.heuristic;
    public static bool operator >(PriorityNode<T> a, PriorityNode<T> b) => a.priority > b.priority ? true : a.priority < b.priority ? false : a.heuristic > b.heuristic;
    public static bool operator <=(PriorityNode<T> a, PriorityNode<T> b) => a < b || a == b;
    public static bool operator >=(PriorityNode<T> a, PriorityNode<T> b) => a > b || a == b;
    public static bool operator ==(PriorityNode<T> a, PriorityNode<T> b) => (a.priority == b.priority && a.heuristic == b.heuristic);
    public static bool operator !=(PriorityNode<T> a, PriorityNode<T> b) => !(a == b);


    public override bool Equals(object obj)
    {
        PriorityNode<T> PriorityNode = obj as PriorityNode<T>;
        if (PriorityNode == null)
            return false;

        return this == PriorityNode;
    }


    public override int GetHashCode() => HashCode.Combine(data, priority);


    public override string ToString()
    {
        return $"Node: {data} (p{priority}, h{heuristic})";
    }
    #endregion
}
