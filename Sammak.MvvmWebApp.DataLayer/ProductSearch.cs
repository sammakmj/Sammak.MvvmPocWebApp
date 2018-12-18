namespace Sammak.MvvmWebApp.DataLayer
{
    public class ProductSearch
    {
        #region Public Properties

        public string ProductName { get; set; }

        #endregion

        #region Constructors

        public ProductSearch() : base()
        {
            Init();
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            // Initialize all search variables
            ProductName = string.Empty;
        }

        #endregion

    }
}
