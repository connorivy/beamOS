namespace BeamOS.WebApp.Client.State;

public class HistoryDeque : HistoryDeque<object> { }

public class HistoryDeque<T>
{
    private readonly LinkedList<T> deque = new();
    private readonly int itemLimit = 50;

    public T PopFirst()
    {
        T obj = this.deque.First();
        this.deque.RemoveFirst();
        return obj;
    }

    public T PopLast()
    {
        T obj = this.deque.Last();
        this.deque.RemoveLast();
        return obj;
    }

    public void PushFirst(T obj)
    {
        this.deque.AddFirst(obj);

        if (this.deque.Count >= itemLimit)
        {
            _ = this.PopLast();
        }
    }
}
