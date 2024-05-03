using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService
{
    public abstract class Validation : ClassINotifyPropertyChanged
    {
        public ValidationErrors ValidationErrors { get; set; }
        public bool IsValid { get; private set; }

        public bool DoesIdAlreadyExist { get; set; }

        protected Validation()
        {
            this.ValidationErrors = new ValidationErrors();
        }

        protected abstract void ValidateSelf();

        public void Validate()
        {
            this.ValidationErrors.Clear();
            this.ValidateSelf();
            this.IsValid = this.ValidationErrors.IsValid;
            this.OnPropertyChanged("IsValid");
            this.OnPropertyChanged("ValidationErrors");
        }
    }
}
