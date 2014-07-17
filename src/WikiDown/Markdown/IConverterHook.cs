namespace WikiDown.Markdown
{
    public interface IConverterHook
    {
        ConverterHookType Type { get; }

        string Apply(string input);
    }
}