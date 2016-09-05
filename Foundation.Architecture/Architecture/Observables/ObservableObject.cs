// Nicholas Ventimiglia 2016-09-05
namespace Foundation.Architecture
{
    /// <summary>
    /// Implements IPropertyChanged for Non-Monobehaviours
    /// </summary>
    public class ObservableObject : IPropertyChanged
    {
        public event PropertyChanged OnPropertyChanged = delegate { };

        public virtual void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }
}