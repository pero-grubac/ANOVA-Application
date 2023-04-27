namespace Lab3
{
    public class IntervalTuple<T, K>
    {
        T Item1;
        K Item2;
        public IntervalTuple(T item1, K item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }
        public T getItem1() { return this.Item1; }
        public K getItem2() { return this.Item2; }
    }
}
