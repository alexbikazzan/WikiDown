using System;

namespace WikiDown.Markdown
{
    public class ServerSideConverterHook : IConverterHook
    {
        private readonly Func<string, string> applyFactory;

        public ServerSideConverterHook(ConverterHookType type, Func<string, string> applyFactory = null)
        {
            this.Type = type;
            this.applyFactory = applyFactory;
        }

        public ConverterHookType Type { get; private set; }

        public virtual string Apply(string html)
        {
            return (this.applyFactory != null) ? this.applyFactory(html) : html;
        }
    }
}