using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// This class represents the pos menu as it is stored on Doshii
    /// </summary>
    public class Menu : ICloneable
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public Menu()
        {
            _products = new List<Product>();
            _surcounts = new List<Surcount>();
            Clear();
        }

        /// <summary>
        /// Resets all property values to default settings.
        /// </summary>
        public void Clear()
        {
            _surcounts.Clear();
            _products.Clear();
        }
        
        private List<Surcount> _surcounts;

        /// <summary>
        /// a list of all the Order level surcounts on the pos menu
        /// </summary>
        public IEnumerable<Surcount> Surcounts
        {
            get
            {
                if (_surcounts == null)
                {
                    _surcounts = new List<Surcount>();
                }
                return _surcounts;
            }
            set { _surcounts = value.ToList<Surcount>(); }
        }

        
        private List<Product> _products;

        /// <summary>
        /// a list of all the products on the pos menu
        /// </summary>
        public IEnumerable<Product> Products
        {
            get
            {
                if (_products == null)
                {
                    _products = new List<Product>();
                }
                return _products;
            }
            set { _products = value.ToList<Product>(); }
        }

        protected bool Equals(Menu other)
        {
            return Equals(_surcounts, other._surcounts) && Equals(_products, other._products);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Menu) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_surcounts != null ? _surcounts.GetHashCode() : 0)*397) ^ (_products != null ? _products.GetHashCode() : 0);
            }
        }

        public object Clone()
        {
            var menu = (Menu)this.MemberwiseClone();
            var surcounts = new List<Surcount>();
            var products = new List<Product>();
            foreach (var surcount in this.Surcounts)
            {
                surcounts.Add((Surcount)surcount.Clone());
            }
            menu.Surcounts = surcounts;
            foreach (var product in this.Products)
            {
                products.Add((Product)product.Clone());
            }
            menu.Products = products;

            return menu;
        }
    }
}
