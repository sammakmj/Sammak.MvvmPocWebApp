using Sammak.MvvmWebApp.DataLayer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace Sammak.MvvmWebApp.ViewModelLayer
{
    public class ProductViewModel
    {
        #region Properties
        public List<Product> DataCollection { get; set; }
        public string Message { get; set; }
        public List<string> Messages { get; set; }
        public ProductSearch SearchEntity { get; set; }
        public string EventAction { get; set; }
        public string EventArgument { get; set; }
        public PDSAPageModeEnum PageMode { get; set; }
        public Product Entity { get; set; }
        public bool IsValid { get; set; }
        #endregion

        #region Constructors

        public ProductViewModel() : base()
        {
            Init();
        }

        #endregion

        #region Public Methods

        public void Init()
        {
            // Initialize properties in this class
            DataCollection = new List<Product>();
            Message = string.Empty;
            Messages = new List<string>();
            SearchEntity = new ProductSearch();
            EventAction = string.Empty;
            EventArgument = string.Empty;
            PageMode = PDSAPageModeEnum.List;
            Entity = new Product();
            IsValid = true;
        }

        public void Publish(Exception ex, string message)
        {
            Publish(ex, message, null);
        }

        public void Publish(Exception ex, string message,
                            NameValueCollection nvc)
        {
            // Update view model properties
            Message = message;
            // TODO: Publish exception here	
        }

        public void HandleRequest()
        {
            // Make sure we have a valid event command
            EventAction = (EventAction == null ? "" :
                           EventAction.ToLower());
            Message = string.Empty;
            switch (EventAction)
            {
                case "add":
                    IsValid = true;
                    PageMode = PDSAPageModeEnum.Add;
                    break;
                case "edit":
                    IsValid = true;
                    PageMode = PDSAPageModeEnum.Edit;
                    GetEntity();
                    break;
                case "search":
                    PageMode = PDSAPageModeEnum.List;
                    break;
                case "resetsearch":
                    PageMode = PDSAPageModeEnum.List;
                    SearchEntity = new ProductSearch();
                    break;
                case "cancel":
                    PageMode = PDSAPageModeEnum.List;
                    break;
                case "save":
                    Save();
                    break;
                case "delete":
                    Delete();
                    break;
            }
            if (PageMode == PDSAPageModeEnum.List)
            {
                BuildCollection();
                if (DataCollection.Count == 0)
                {
                    Message = "No Product Data Found.";
                }
            }
        }
        #endregion

        #region Private/Protected Methods

        protected virtual void GetEntity()
        {
            MvvmData db = null;
            try
            {
                db = new MvvmData();
                // Get the entity
                if (!string.IsNullOrEmpty(EventArgument))
                {
                    Entity = db.Products.Find(Convert.ToInt32(EventArgument));
                }
            }
            catch (Exception ex)
            {
                Publish(ex, "Error Retrieving Product With ID="
                             + EventArgument);
            }
        }

        protected void BuildCollection()
        {
            MvvmData db = null;
            try
            {
                db = new MvvmData();
                // Get the collection
                DataCollection = db.Products.ToList();

                // Filter the collection
                if (DataCollection != null && DataCollection.Count > 0)
                {
                    if (!string.IsNullOrEmpty(SearchEntity.ProductName))
                    {
                        DataCollection = DataCollection.FindAll(
                          p => p.ProductName
                            .StartsWith(SearchEntity.ProductName,
                               StringComparison.InvariantCultureIgnoreCase));
                    }
                }
                if (DataCollection.Count == 0)
                {
                    Message = "No Product Data Found.";
                }
            }
            catch (Exception ex)
            {
                Publish(ex, "Error while loading products.");
            }
        }

        protected void ValidationErrorsToMessages(DbEntityValidationException ex)
        {
            foreach (DbEntityValidationResult result in ex.EntityValidationErrors)
            {
                foreach (DbValidationError item in result.ValidationErrors)
                {
                    Messages.Add(item.ErrorMessage);
                }
            }
        }

        protected void Insert()
        {
            MvvmData db = null;
            try
            {
                db = new MvvmData();
                // Do editing here
                db.Products.Add(Entity);
                db.SaveChanges();
                PageMode = PDSAPageModeEnum.List;
            }
            catch (DbEntityValidationException ex)
            {
                IsValid = false;
                ValidationErrorsToMessages(ex);
            }
            catch (Exception ex)
            {
                Publish(ex, "Error Inserting New Product: '"
                            + Entity.ProductName + "'");
            }
        }

        protected void Update()
        {
            MvvmData db = null;
            try
            {
                db = new MvvmData();
                // Do editing here
                db.Entry(Entity).State = EntityState.Modified;
                db.SaveChanges();
                PageMode = PDSAPageModeEnum.List;
            }
            catch (DbEntityValidationException ex)
            {
                IsValid = false;
                ValidationErrorsToMessages(ex);
            }
            catch (Exception ex)
            {
                Publish(ex, "Error Updating Product With ID="
                             + Entity.ProductId.ToString());
            }
        }

        public virtual void Delete()
        {
            MvvmData db = null;
            try
            {
                db = new MvvmData();
                if (!string.IsNullOrEmpty(EventArgument))
                {
                    Entity =
                     db.Products.Find(Convert.ToInt32(EventArgument));
                    db.Products.Remove(Entity);
                    db.SaveChanges();
                    PageMode = PDSAPageModeEnum.List;
                }
            }
            catch (Exception ex)
            {
                Publish(ex, "Error Deleting Product With ID="
                             + Entity.ProductName);
            }
        }

        protected void Save()
        {
            IsValid = true;
            if (PageMode == PDSAPageModeEnum.Add)
            {
                Insert();
            }
            else
            {
                Update();
            }
        }
        #endregion

    }
}
