namespace WikiDown.Website.ApiModels
{
    public class IdTextApiModel : IdTextApiModel<int>
    {
        public IdTextApiModel()
        {
        }

        public IdTextApiModel(int id, string text)
        {
            this.Id = id;
            this.Text = text;
        }
    }

    public class IdTextApiModel<TId>
    {
        public IdTextApiModel()
        {
        }

        public IdTextApiModel(TId id, string text)
        {
            this.Id = id;
            this.Text = text;
        }

        public TId Id { get; set; }

        public string Text { get; set; }
    }
}