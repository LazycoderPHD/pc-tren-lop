namespace OSWorkbench
{
    public class ThreadInfo
    {
        public int ThreadId { get; set; }
        public string StateText { get; set; } = "";
        public string PriorityText { get; set; } = "";
        public string StartTimeText { get; set; } = "";
    }
}