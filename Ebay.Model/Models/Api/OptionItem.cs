namespace Ebay.Model.Models.Api
{
    public class OptionItem
    {
        public string Label { get; set; }
        public int Value { get; set; }
        public bool Selected { get; set; }
    }

    public class OptionItem<T>
    {
        public string Label { get; set; }
        public T Value { get; set; }
    }

    public class OptionData<TValue, TData>
    {
        public string Label { get; set; }
        public TValue Value { get; set; }
        public TData Data { get; set; }
    }
}