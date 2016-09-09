// Nicholas Ventimiglia 2016-09-05

namespace Foundation.Architecture
{
    /// <summary>
    /// Action
    /// </summary>
    /// <param name="memberName"></param>
    public delegate void PropertyChanged(string memberName);

    /// <summary>
    /// Property Change observation
    /// </summary>
    public interface IPropertyChanged 
    {
        event PropertyChanged OnPropertyChanged;
        void RaisePropertyChanged(string propertyName);
    }
    
}