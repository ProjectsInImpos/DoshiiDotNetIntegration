using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoshiiDotNetIntegration.Models
{
    /// <summary>
    /// represents an app that a member is registered with. 
    /// </summary>
    public class App : ICloneable
    {
        public App()
        {
            _types = new List<string>();
        }

        /// <summary>
        /// the Id of the partner app
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// the name of the partner app
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the amount of points available for the member on this app
        /// </summary>
        public decimal Points { get; set; }

        public string Ref { get; set; }
        
        private List<string> _types;

        public List<string> Types
        {
            get
            {
                if (_types == null)
                {
                    _types = new List<string>();
                }
                return _types;
            }
            set { _types = value.ToList<string>(); }
        }

        protected void Clear()
        {
           this._types.Clear();
           this.Id = String.Empty;
           this.Name = string.Empty;
           this.Points = 0;
        }

        public object Clone()
        {
            var app = (App)this.MemberwiseClone();
            var types = new List<string>();
            foreach (var type in this.Types)
            {
                types.Add((string)type.Clone());
            }
            app.Types = types;

            return app;
        }

        protected bool Equals(App other)
        {
            return string.Equals(Id, other.Id) && string.Equals(Name, other.Name) && Ref == other.Ref;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((App) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Ref.GetHashCode();
                return hashCode;
            }
        }
    }
}
