using UnityEngine;

namespace Foundation.Architecture
{
    /// <summary>
    /// Implements IPropertyChanged for MonoBehaviour
    /// </summary>
    public class ObservableBehaviour : MonoBehaviour, IPropertyChanged
    {
        public event PropertyChanged OnPropertyChanged = delegate { };
        
        public virtual void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
    }
}